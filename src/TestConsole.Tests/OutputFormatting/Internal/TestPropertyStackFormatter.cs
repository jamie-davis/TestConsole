using System;
using System.Linq;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsole.Tests.OutputFormatting.UnitTestutilities;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    public class TestPropertyStackFormatter
    {
        private static readonly string End = Environment.NewLine + "End";
        const int ColumnWidth = 20;
        private SplitCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new SplitCache();
        }

        [Test]
        public void FormatHeadingIsUsedAsThePropertyName()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading"), "Value", ColumnWidth, _cache);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void LongValuesAreWordWrapped()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading"), "A nice long value that can only be accomodated with word wrapping.", ColumnWidth, _cache);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void AllPartsOfARightAlignedValueAreRightAligned()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Heading", alignment: ColumnAlign.Right), "A nice long value that can only be accomodated with word wrapping.", ColumnWidth, _cache);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void FormattingAcceptsValueWhereFirstWordDoesNotFitOnFirstLine()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Format heading", alignment: ColumnAlign.Right), "Firstwordislong", ColumnWidth, _cache);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void FirstLineHangingIndentIsRespected()
        {
            var output = PropertyStackFormatter.Format(new ColumnFormat("Format heading", alignment: ColumnAlign.Right), "Value words words", ColumnWidth, _cache, firstLineHangingIndent: 10);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }

        [Test]
        public void RenderableElementsAreStacked()
        {
            var recorder = new RecordingConsoleAdapter();
            recorder.WriteLine("Some simple text");
            recorder.FormatTable(Enumerable.Range(0,5).Select(i => new {Index = i}));
            var output = PropertyStackFormatter.Format(new ColumnFormat("Format heading", 
                alignment: ColumnAlign.Right), 
                recorder, 
                ColumnWidth,
                _cache);
            var text = RulerFormatter.MakeRuler(ColumnWidth) + Environment.NewLine + string.Join(Environment.NewLine, output) + End;
            Console.WriteLine(text);
            Approvals.Verify(text);
        }
    }
}