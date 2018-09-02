using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsole.Tests.TestingUtilities;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestDefaultWidthCalculator
    {
        private List<ColumnFormat> _formats;

        [SetUp]
        public void SetUp()
        {
            SetUpTests.OverrideCulture();
            _formats = new List<ColumnFormat>
            {
                MakeFormat<sbyte>(),
                MakeFormat<byte>(),
                MakeFormat<short>(),
                MakeFormat<ushort>(),
                MakeFormat<int>(),
                MakeFormat<uint>(),
                MakeFormat<long>(),
                MakeFormat<ulong>(),
                MakeFormat<decimal>(),
                MakeFormat<float>(),
                MakeFormat<double>(),
                MakeFormat<string>(),
                MakeFormat<char>(),
                MakeFormat<DateTime>(),
                MakeFormat<TimeSpan>(),
                MakeFormat<bool>(),
                MakeFormat<object>(),
            };
        }

        private ColumnFormat MakeFormat<T>()
        {
            return new ColumnFormat("A", typeof(T));
        }

        [Test]
        public void DefaultMinWidthIsAsExpected()
        {
            var report = AllFormats(f => DefaultWidthCalculator.Min(f).ToString(CultureInfo.InvariantCulture));
            Approvals.Verify(report);
        }

        [Test]
        public void DefaultMaxWidthIsAsExpected()
        {
            var report = AllFormats(f => DefaultWidthCalculator.Max(f).ToString(CultureInfo.InvariantCulture));
            Approvals.Verify(report);
        }

        private string AllFormats(Func<ColumnFormat, string> func)
        {
            var report = string.Join(Environment.NewLine,
                _formats.Select(f => string.Format("{0,-8} = {1}", f.Type.Name, func(f))));
            Console.WriteLine(report);
            return report;
        }
    }
}