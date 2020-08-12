using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TestConsoleLib.Testing
{
    internal static class StackInterpreter
    {
        internal static CallerStackFrameInfo GetCallerStackFrameInfo(string callerMethod, string callerPath, params Type[] ignoreTypes)
        {
            var stackTrace = new StackTrace(true);
            Debug.Assert(stackTrace != null);
            var stackFrames = StackFrameFilter.Filtered(stackTrace, ignoreTypes, typeof(StackInterpreter)).ToList();

            var stackFrame = stackFrames.FirstOrDefault();
            if (stackFrame == null) return null;

            if (!TryAsyncCsCall(stackFrames, callerMethod, callerPath, out var info))
            {
                var path = Path.GetDirectoryName(stackFrame.GetFileName());
                var method = stackFrame.GetMethod();
                info = new CallerStackFrameInfo(method.DeclaringType, method, path);
            }

            return info;
        }

        private static Regex _csAsyncDirectRegex = new Regex(@"^<(.*)>b__[0-9]*$", RegexOptions.Compiled);
        private static Regex _csAsyncDeclaringRegex = new Regex(@"^<(.*)>d__[0-9]*$", RegexOptions.Compiled);
        private static Regex _csNestedLambdaRegex = new Regex(@"^<>c__DisplayClass[0-9]*_[0-9]$", RegexOptions.Compiled);

        private static bool TryAsyncCsCall(List<StackFrame> stackFrames, string callerMethod, string callerPath, out CallerStackFrameInfo info)
        {
            var sf = stackFrames[0];
            var method = sf.GetMethod();
            if (TryDirect(sf, method, out info)
                || TryMoveNext(sf, method, stackFrames, callerMethod, callerPath, out info))
                return true;
            info = null;
            return false;
        }

        private static bool TryDirect(StackFrame sf, MethodBase method, out CallerStackFrameInfo info)
        {
            var match = _csAsyncDirectRegex.Match(method.Name);
            if (match.Success)
            {
                var path = sf.GetFileName();
                var methodName = match.Result("$1");
                var declaringType = method.DeclaringType;

                while (declaringType.IsNestedPrivate && _csNestedLambdaRegex.IsMatch(declaringType.Name))
                    declaringType = declaringType.DeclaringType;

                if (path != null && declaringType != null && !string.IsNullOrWhiteSpace(methodName))
                {
                    var asyncMethod = declaringType.GetMethod(methodName);
                    if (asyncMethod != null)
                    {
                        info = new CallerStackFrameInfo(declaringType, asyncMethod, path);
                        return true;
                    }
                }
            }

            info = null;
            return false;
        }

        private static bool TryMoveNext(StackFrame sf, MethodBase method, List<StackFrame> stackFrames, string callerMethod, string callerPath, out CallerStackFrameInfo info)
        {
            if (method.DeclaringType == null)
            {
                info = null;
                return false;
            }

            var match = _csAsyncDeclaringRegex.Match(method.DeclaringType.Name);
            if (match.Success)
            {
                var path = sf.GetFileName();
                var methodName = match.Result("$1");

                foreach (var stackFrame in stackFrames)
                {
                    var candidateType = stackFrame.GetMethod()?.DeclaringType;
                    MethodBase candidateMethod = null;
                    while (candidateType != null && (candidateMethod = candidateType.GetMethod(methodName)) == null && candidateType.IsNested)
                        candidateType = candidateType.DeclaringType;
                    if (candidateMethod != null)
                    {
                        info = new CallerStackFrameInfo(candidateType, candidateMethod, Path.GetDirectoryName(path));
                        return true;
                    }
                }
            }
            info = null;
            return false;
        }
    }
}