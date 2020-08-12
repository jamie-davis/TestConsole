using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestConsoleLib.Testing
{
    internal static class StackFrameFilter
    {
        internal static IEnumerable<StackFrame> Filtered(StackTrace stackTrace, Type[] ignoreTypes, params Type[] moreIgnoreTypes)
        {
            var stackFrames = stackTrace.GetFrames();

            if (stackFrames == null)
                yield break;

            foreach (var sf in stackFrames)
            {
                var method = sf.GetMethod();
                var declaringType = method?.DeclaringType;
                if (declaringType != null && ignoreTypes.Contains(declaringType) || moreIgnoreTypes.Contains(declaringType))
                    continue;

                yield return sf;
            }
        }
    }
}