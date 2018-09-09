using System;

namespace TestConsoleLib.Exceptions
{
    public class UnableToInvokeFileCompare : Exception
    {
        public UnableToInvokeFileCompare(Exception exception) : base("Unable to invoke a file compare utility due to exception.", exception)
        {
        }
    }
}