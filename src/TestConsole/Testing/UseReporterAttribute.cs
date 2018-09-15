using System;
// ReSharper disable All

namespace ApprovalTests.Reporters
{
    /// <summary>
    /// This exists for compilation compatibility with ApprovalTests, hence the namespace.
    /// </summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Class)]
    public class UseReporterAttribute : Attribute
    {
        public Type ReporterType { get; }

        public UseReporterAttribute(Type reporterType)
        {
            ReporterType = reporterType;
        }
    }
}