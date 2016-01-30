using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TestConsoleLib.CallRecording.Internals;

namespace TestConsoleLib.CallRecording
{
    /// <summary>
    /// Sometimes all a test needs to do is confirm that the expected calls are made to
    /// an interface by the class under test.
    /// <para/>
    /// This class generates a call reporting wrapper around an interface implementation.
    /// Calls made to the interface via the wrapper will be reported on the supplied
    /// <see cref="Output"/> instance, and can be verified in the unit test.
    /// </summary>
    public static class CallRecorder
    {
        private static int _nextTypeIndex = 1;
        private static readonly MethodInfo OutputWrapLine = typeof(Output).GetMethod("WrapLine");
        private static readonly MethodInfo OutputParameter = typeof(CallRecorderPrinting).GetMethod("PrintParameter");
        private static readonly MethodInfo OutputRefParameterReturned = typeof(CallRecorderPrinting).GetMethod("PrintRefParameterReturnValue");
        private static readonly MethodInfo OutputReturnValue = typeof(CallRecorderPrinting).GetMethod("PrintReturnValue");
        private static readonly MethodInfo OutputException = typeof(CallRecorderPrinting).GetMethod("PrintException");
        private static readonly MethodInfo OutputMethodCall = typeof(CallRecorderPrinting).GetMethod("PrintMethodCall");

        static CallRecorder()
        {
            Debug.Assert(OutputWrapLine != null);
            Debug.Assert(OutputParameter != null);
            Debug.Assert(OutputRefParameterReturned != null);
            Debug.Assert(OutputReturnValue != null);
            Debug.Assert(OutputMethodCall != null);
            Debug.Assert(OutputException != null);
        }

        /// <summary>
        /// Generate a recording wrapper for an instance.
        /// </summary>
        /// <param name="impl">The instance to be wrapped.</param>
        /// <param name="output">The <see cref="Output"/> to receive the recorded log.</param>
        /// <typeparam name="T">The type being wrapped.</typeparam>
        /// <returns>A wrapper class instance.</returns>
        public static T Generate<T>(T impl, Output output) where T : class
        {
            AssemblyBuilder assemblyBuilder;
            var builder = MakeTypeBuilder(typeof(T), out assemblyBuilder);
            var outputField = MakeField(builder, typeof (Output));
            var wrappedInstanceField = MakeField(builder, typeof (T));
            foreach (var method in typeof(T).GetMethods())
            {
                var methodAttributes = method.Attributes ^ MethodAttributes.Abstract;
                var parameterTypes = method.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToArray();
                var methodBuilder = builder.DefineMethod(method.Name, methodAttributes, 
                    method.ReturnType, parameterTypes);
                BuildMethod(method, methodBuilder, outputField, wrappedInstanceField, builder);
            }

            GenerateConstructor<T>(builder, outputField, wrappedInstanceField);

            var builtType = builder.CreateType();

            return (T)Activator.CreateInstance(builtType, new object[] { impl, output });
        }

        private static FieldInfo MakeField(TypeBuilder builder, Type fieldType)
        {
            var fieldBuilder = builder.DefineField("f_" + fieldType.Name, fieldType, FieldAttributes.Private);
            return fieldBuilder;
        }

        private static void GenerateConstructor<T>(TypeBuilder typeBuilder, FieldInfo outputField, FieldInfo wrappedInstanceField)
        {
            MethodAttributes ctorAttributes = MethodAttributes.Public;
            var builder = typeBuilder.DefineConstructor(ctorAttributes, CallingConventions.Standard, new[] {typeof (T), typeof(Output)});
            var il = builder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, outputField);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, wrappedInstanceField);
            il.Emit(OpCodes.Ret);
        }

        private static void BuildMethod(MethodInfo method, MethodBuilder builder, FieldInfo outputField, FieldInfo instanceField, TypeBuilder typeBuilder)
        {
            var il = builder.GetILGenerator();
            var exit = il.BeginExceptionBlock();

            EmitPrintCall(method, outputField, instanceField, il);
            EmitWrapperCall(method, instanceField, il);
            EmitReturnValueCall(method, outputField, il);
            EmitRefParameterValueCalls(il, method, outputField);
            il.Emit(OpCodes.Ret);
            //il.Emit(OpCodes.Leave, exit);

            il.BeginCatchBlock(typeof(Exception));
            //il.Emit(OpCodes.Nop);
            //EmitCatch(il, method, outputField);
            //il.Emit(OpCodes.Throw);
            il.EndExceptionBlock();
            il.Emit(OpCodes.Ret);
        }

        private static void EmitCatch(ILGenerator il, MethodInfo method, FieldInfo outputField)
        {
            OpCode storeOpCode, loadOpCode;
            if (method.ReturnType == typeof (void))
            {
                storeOpCode = OpCodes.Stloc_0;
                loadOpCode = OpCodes.Ldloc_0;
            }
            else
            {
                storeOpCode = OpCodes.Stloc_1;
                loadOpCode = OpCodes.Ldloc_1;
            }

            il.DeclareLocal(typeof(object));
            il.Emit(storeOpCode);
            il.Emit(loadOpCode);

            //il.Emit(OpCodes.Ldstr, method.Name);
            //il.Emit(OpCodes.Ldarg_0);
            //il.Emit(OpCodes.Ldfld, outputField);
            //il.Emit(OpCodes.Call, OutputException);

            //il.Emit(loadOpCode);
            il.Emit(OpCodes.Throw);
        }

        private static void EmitReturnValueCall(MethodInfo method, FieldInfo outputField, ILGenerator il)
        {
            var returnType = method.ReturnType;
            if (returnType == typeof (void))
                return;

            il.DeclareLocal(returnType);

            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);

            var printMethod = OutputReturnValue.MakeGenericMethod(returnType);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, outputField);
            il.Emit(OpCodes.Call, printMethod);
            il.Emit(OpCodes.Ldloc_0);
        }

        private static Type GetDirectType(Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        private static void EmitPrintCall(MethodInfo method, FieldInfo outputField, FieldInfo instanceField, ILGenerator il)
        {
            var parameters = method.GetParameters()
                .Select(p => string.Format("{0} {1}", p.ParameterType, p.Name));
            var parameterList = string.Join(", ", parameters);
            
            il.Emit(OpCodes.Ldstr, method.Name);
            il.Emit(OpCodes.Ldstr, parameterList);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, outputField);
            il.Emit(OpCodes.Call, OutputMethodCall);

            EmitParameterPrintCalls(il, method, outputField);
        }

        private static void EmitParameterPrintCalls(ILGenerator il, MethodInfo method, FieldInfo outputField)
        {
            var index = 1;
            foreach (var parameterInfo in method.GetParameters())
            {
                var directType = GetDirectType(parameterInfo.ParameterType);
                var parameterType = parameterInfo.ParameterType;
                il.Emit(OpCodes.Ldarg, index);
                var isByRef = parameterType.IsByRef;
                if (isByRef)
                {
                    //dereference the argument to get the value
                    il.Emit(GetLoadOp(directType));
                }

                var printMethod = OutputParameter.MakeGenericMethod(directType);
                
                il.Emit(OpCodes.Ldstr, parameterInfo.Name);
                il.Emit(OpCodes.Ldc_I4, index++);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, outputField);
                il.Emit(isByRef ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Call, printMethod);
            }
        }

        private static void EmitRefParameterValueCalls(ILGenerator il, MethodInfo method, FieldInfo outputField)
        {
            var index = 1;
            var refParams = method.GetParameters().Where(p => p.ParameterType.IsByRef);
            foreach (var parameterInfo in refParams)
            {
                var directType = GetDirectType(parameterInfo.ParameterType);
                var parameterType = parameterInfo.ParameterType;
                il.Emit(OpCodes.Ldarg, index);
                il.Emit(GetLoadOp(directType));

                var printMethod = OutputRefParameterReturned.MakeGenericMethod(directType);
                
                il.Emit(OpCodes.Ldstr, parameterInfo.Name);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, outputField);
                il.Emit(OpCodes.Call, printMethod);
            }
        }

        private static OpCode GetLoadOp(Type parameterType)
        {
            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.DateTime:
                case TypeCode.Object:
                    return OpCodes.Ldind_Ref;

                case TypeCode.Byte:
                case TypeCode.SByte:
                    return OpCodes.Ldind_I1;

                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return OpCodes.Ldind_I2;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return OpCodes.Ldind_I8;

                case TypeCode.Single:
                    return OpCodes.Ldind_R4;

                case TypeCode.Double:
                    return OpCodes.Ldind_R8;

                default:
                    return OpCodes.Ldind_I4;
            }
        }

        private static void EmitWrapperCall(MethodInfo method, FieldInfo instanceField, ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, instanceField);

            var index = 1;
            foreach (var parameter in method.GetParameters())
            {
                  il.Emit(OpCodes.Ldarg, index++);
            }
            il.Emit(OpCodes.Callvirt, method);
        }

        private static TypeBuilder MakeTypeBuilder(Type baseType, out AssemblyBuilder assembly)
        {
            var assemblyName = new AssemblyName("TestConsoleWrappers");
            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assembly.DefineDynamicModule("Module");
            var typeBuilder = ApplyBaseType(baseType, moduleBuilder);
            return typeBuilder;
        }

        private static TypeBuilder ApplyBaseType(Type baseType, ModuleBuilder moduleBuilder)
        {
            if (baseType == null)
            {
                return moduleBuilder.DefineType("T" + _nextTypeIndex++ + "-Recorder",
                    TypeAttributes.Class);
            }

            if (baseType.IsInterface)
            {
                var typeBuilder = moduleBuilder.DefineType(baseType.Name + "-Recorder",
                    TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(baseType);
                return typeBuilder;
            }

            return moduleBuilder.DefineType(baseType.Name + "-Recorder",
                TypeAttributes.Class, baseType);
        }
    }
}
