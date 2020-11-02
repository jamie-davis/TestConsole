using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsole.Tests.OutputFormatting.UnitTestutilities;
using TestConsole.Tests.TestingUtilities;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    public class TestPropertyStackColumnFormatter
    {
        private TestType _data;
        private List<PropertyColumnFormat> _cols;
        private static readonly string Ruler = RulerFormatter.MakeRuler(80);
        private SplitCache _cache;

        class TestType
        {
            public string String1 { get; set; }
            public string String2 { get; set; }
            public double Double { get; set; }
            public DateTime DateTime { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _cache = new SplitCache();
            SetUpTests.OverrideCulture();
            _data = new TestType
            {
                String1 = "A simple string value",
                String2 = "LongValueWord short",
                Double = Math.PI,
                DateTime = DateTime.Parse("2014-05-05 19:23:52")
            };
            _cols = FormatAnalyser.Analyse(_data.GetType(), null, true);
        }

        [Test]
        public void SingleColumnIsFormatted()
        {
            var report = PropertyStackColumnFormatter.Format(_cols.Take(1), _data, 30, _cache);
            var output = BuildOutput(report, 30);
            Approvals.Verify(output);
        }

        [Test]
        public void SingleColumnIsWrapped()
        {
            var report = PropertyStackColumnFormatter.Format(_cols.Take(1), _data, 15, _cache);
            var output = BuildOutput(report, 15);
            Approvals.Verify(output);
        }

        [Test]
        public void MultipleColumnsAreFormatted()
        {
            var report = PropertyStackColumnFormatter.Format(_cols, _data, 30, _cache);
            var output = BuildOutput(report, 30);
            Approvals.Verify(output);
        }

        [Test]
        public void MultipleColumnsAreWrapped()
        {
            var report = PropertyStackColumnFormatter.Format(_cols, _data, 15, _cache);
            var output = BuildOutput(report, 15);
            Approvals.Verify(output);
        }

        [Test]
        public void SingleColumnIsFormattedWithLiteralValue()
        {
            var report = PropertyStackColumnFormatter.Format(_cols.Take(1), new[] {"pre formatted string"}, 30, _cache);
            var output = BuildOutput(report, 30);
            Approvals.Verify(output);
        }

        [Test]
        public void SingleColumnIsWrappedWithLiteralValue()
        {
            var report = PropertyStackColumnFormatter.Format(_cols.Take(1), new[] { "pre formatted string" }, 15, _cache);
            var output = BuildOutput(report, 15);
            Approvals.Verify(output);
        }

        [Test]
        public void MultipleColumnsAreFormattedWithLiteralValues()
        {
            var preFormattedValues = new[]
            {
                "pre formatted string",
                "pre-formatted words",
                "3.14pf",
                "06/05/2014 20:46:25"
            };
            var report = PropertyStackColumnFormatter.Format(_cols, preFormattedValues, 30, _cache);
            var output = BuildOutput(report, 30);
            Approvals.Verify(output);
        }

        [Test]
        public void MultipleColumnsAreWrappedWithLiteralValues()
        {
            var preFormattedValues = new[]
            {
                "pre formatted string",
                "pre-formatted words",
                "3.14pf",
                "06/05/2014 20:46:25"
            };
            var report = PropertyStackColumnFormatter.Format(_cols, preFormattedValues, 15, _cache);
            var output = BuildOutput(report, 15);
            Approvals.Verify(output);
        }

        [Test]
        public void FirstLineHangingIndentAppliesToFirstPropertyOnly()
        {
            var preFormattedValues = new[]
            {
                "pre formatted string",
                "pre-formatted words",
                "3.14pf",
                "06/05/2014 20:46:25"
            };
            var report = PropertyStackColumnFormatter.Format(_cols, preFormattedValues, 15, _cache, firstLineHangingIndent: 5);
            var output = BuildOutput(report, 15);
            Approvals.Verify(output);
        }

        private static string BuildOutput(IEnumerable<string> report, int length)
        {
            var output = string.Join(Environment.NewLine, new[] {Ruler.Substring(0, length)}.Concat(report));
            Console.WriteLine(output);
            return output;
        }
    }
}