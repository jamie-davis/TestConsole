using System;

namespace TestConsoleLib.Testing
{
    public class ApprovalsReporterAttribute : Attribute
    {
        public ApprovalsReporterAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}