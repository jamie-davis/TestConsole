using System;

namespace TestConsoleLib.Exceptions
{
    public class UnableToInvokeFileCompare : Exception
    {
        public string Path { get; } = Environment.GetEnvironmentVariable("path");
        public UnableToInvokeFileCompare(Exception exception) : base("Unable to invoke a file compare utility due to exception.", exception)
        {
        }
    }
}