using System;

namespace TestConsoleLib.Exceptions
{
    public class UnableToLocateApprovedFileException : Exception
    {
        public UnableToLocateApprovedFileException() : base("Unable to determine location for the approved file. This is usually because the tests have been compiled in release mode.")
        {
            
        }
    }
}