using System;
using System.Reflection;

namespace TestConsoleLib.Testing
{
    internal class CallerStackFrameInfo
    {
        internal Type Type { get; }
        internal MethodBase Method { get; }
        internal string Path { get; }

        internal CallerStackFrameInfo(Type type, MethodBase method, string path)
        {
            Type = type;
            Method = method;
            Path = path;
        }
    }
}