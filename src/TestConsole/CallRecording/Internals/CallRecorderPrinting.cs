using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestConsoleLib.CallRecording.Internals
{
    public class CallRecorderPrinting
    {
        /// <summary>
        /// Internal function to print a parameter value.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="output">The buffer to print to.</param>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        public static void PrintParameter<T>(T value, string parameterName, int parameterIndex, Output output, bool isByRef)
        {
            var refDesc = isByRef ? "ref " : string.Empty;
            if (Type.GetTypeCode(typeof(T)) != TypeCode.Object)
                output.WrapLine("{0}{1}. {2}={3}", refDesc, parameterIndex, parameterName, value);
            else
            {
                output.WrapLine("{0}{1}. {2}", refDesc, parameterIndex, parameterName);
                output.WriteLine();
                output.FormatObject(value);
            }
        }

        /// <summary>
        /// Internal function to print a return value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="output">The buffer to print to.</param>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        public static void PrintReturnValue<T>(T value, Output output)
        {
            output.WriteLine();
            if (Type.GetTypeCode(typeof(T)) != TypeCode.Object)
                output.WrapLine("Returned {0}",value);
            else
            {
                output.WrapLine("Returned");
                output.WriteLine();
                output.FormatObject(value);
            }
        }

        /// <summary>
        /// Internal function to print a return value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="output">The buffer to print to.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        public static void PrintRefParameterReturnValue<T>(T value, string parameterName, Output output)
        {
            if (Type.GetTypeCode(typeof(T)) != TypeCode.Object)
                output.WrapLine("ref {0} Returned {1}",parameterName, value);
            else
            {
                output.WrapLine("ref {0} Returned", parameterName);
                output.WriteLine();
                output.FormatObject(value);
            }
        }

        /// <summary>
        /// Internal function to print a method call.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">The string representing the parameters of the method.</param>
        /// <param name="output">The buffer to print to.</param>
        public static void PrintMethodCall(string methodName, string parameters, Output output)
        {
            output.WrapLine("Call to {0}({1})", methodName, parameters);
        }

        /// <summary>
        /// Internal function to print an exception throwb from a method call.
        /// </summary>
        /// <param name="exception">The exception object.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="output">The buffer to print to.</param>
        public static void PrintException(object exception, string methodName, Output output)
        {
            output.WrapLine("Exception {0} {1}", methodName, exception);
        }
    }
}
