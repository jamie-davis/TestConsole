using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    public class TestColumnWrapper
    {
        private SplitCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new SplitCache();
        }

        [Test]
        public void StringIsWrappedAtWordBreaks()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void NoTrailingSpacesAreIncludedOnLineWrap()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four\t\t\t\t\t\t\t\t          five\tsix seven eight nine ten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void WordsLongerThanOneLineAreBroken()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four fivesixseveneightnineteneleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void VeryLongInitialWordisBroken()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "Onetwothreefourfivesixseveneightnineten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LineBreaksArePreserved()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three\r\nfour five six seven eight\r\n\r\n\r\nnine\r\nten\r\neleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void WordBreaksAreCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20, _cache);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20, _cache)));
            Assert.That(addedBreaks, Is.EqualTo(2));
        }

        [Test]
        public void TrailingSpacesDoNotCauseAdditionalLineBreaks()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four\t\t\t\t\t\t\t\t          five\tsix seven eight nine ten eleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20, _cache);
            Console.WriteLine("----+----|----+----|");
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20, _cache)));
            Assert.That(addedBreaks, Is.EqualTo(3));
        }

        [Test]
        public void BreaksInVeryLongWordsAreCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three four fivesixseveneightnineteneleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20, _cache);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20, _cache)));
            Assert.That(addedBreaks, Is.EqualTo(2));
        }

        [Test]
        public void BreaksInVeryLongInitialWordAreCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "Onetwothreefourfivesixseveneightnineten eleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20, _cache);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20, _cache)));
            Assert.That(addedBreaks, Is.EqualTo(2));
        }

        [Test]
        public void DataLineBreaksAreNotCounted()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three\r\nfour five six seven eight\r\n\r\n\r\nnine\r\nten\r\neleven.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20, _cache);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20, _cache)));
            Assert.That(addedBreaks, Is.EqualTo(1));
        }

        [Test]
        public void SingleLineDataAddsNoLineBreaks()
        {
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three.";
            var addedBreaks = ColumnWrapper.CountWordwrapLineBreaks(value, c, 20, _cache);
            Console.WriteLine(string.Join("\r\n", ColumnWrapper.WrapValue(value, c, 20, _cache)));
            Assert.That(addedBreaks, Is.EqualTo(0));
        }

        [Test]
        public void LinesAreExpandedToCorrectLength()
        {
            var c = new ColumnFormat("h", typeof (string));
            c.SetActualWidth(20);
            var value = "Line" + Environment.NewLine + "Data" + Environment.NewLine + "More";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache).Select(l => "-->" + l + "<--");
            var result = FormatResult(value, wrapped, 20, 3);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void RightAlignedLinesAreExpandedToCorrectLength()
        {
            var c = new ColumnFormat("h", typeof (string), ColumnAlign.Right);
            c.SetActualWidth(20);
            var value = "Line" + Environment.NewLine + "Data" + Environment.NewLine + "More";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache).Select(l => "-->" + l + "<--");
            var result = FormatResult(value, wrapped, 20, 3);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void FirstLineIndentShortensFirstLine()
        {
            var c = new ColumnFormat("h", typeof(string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache, firstLineHangingIndent: 10);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LeadingSpacesArePreserved()
        {
            var c = new ColumnFormat("h", typeof(string));
            const string value = " One two three.";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void TrailingSpacesArePreserved()
        {
            var c = new ColumnFormat("h", typeof(string));
            const string value = "One two three. ";
            var wrapped = ColumnWrapper.WrapValue(value, c, 20, _cache);
            var result = FormatResult(value, wrapped, 20);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void WrapAndMeasureWordsCanWorkWithSplitData()
        {
            //Arrange
            var c = new ColumnFormat("h", typeof(string));
            const string value = "One two three four five six seven eight nine ten eleven.";
            var words = WordSplitter.Split(value, 4);

            //Act
            int wrappedLines;
            var wrapped = ColumnWrapper.WrapAndMeasureWords(words, c, 20, 0, out wrappedLines);

            //Assert
            var result = FormatResult(value, wrapped, 20)
                + Environment.NewLine
                + string.Format("Added line breaks = {0}", wrappedLines);
            Console.WriteLine(result);
            Approvals.Verify(result);
        }

        [Test]
        public void LongestLineIsComputed()
        {
            //Arrange
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three\r\nfour five six seven eight\r\n\r\n\r\nnine\r\nten\r\neleven.";

            //Act
            var longest = ColumnWrapper.GetLongestLineLength(value, c, _cache);

            //Assert
            longest.Should().Be(25);
        }

        [Test]
        public void LongestLineCanBeEven()
        {
            //Arrange
            var c = new ColumnFormat("h", typeof (string));
            const string value = "One two three\r\nXfour five six seven eight\r\n\r\n\r\nnine\r\nten\r\neleven.";

            //Act
            var longest = ColumnWrapper.GetLongestLineLength(value, c, _cache);

            //Assert
            longest.Should().Be(26);
        }

        [Test]
        public void LongestLineWhenTextIsEmptyIsOne()
        {
            //Arrange
            var c = new ColumnFormat("h", typeof (string));

            //Act
            var longest = ColumnWrapper.GetLongestLineLength(string.Empty, c, _cache);

            //Assert
            longest.Should().Be(1);
        }

        private string FormatResult(string value, IEnumerable<string> wrapped, int guideWidth, int indent = 0)
        {
            var indentString = new string(' ', indent);
            var sb = new StringBuilder();
            sb.AppendLine(TestContext.CurrentContext.Test.Name);
            sb.AppendLine();

            sb.AppendLine("Original:");
            sb.AppendLine(value);
            sb.AppendLine();

            sb.AppendLine("Wrapped:");
            var guide = Enumerable.Range(0, guideWidth)
                .Select(i => ((i + 1)%10).ToString())
                .Aggregate((t, i) => t + i);
            var divide = Enumerable.Range(0, guideWidth)
                .Select(i => ((i + 1) % 10) == 0 ? "+" : "-")
                .Aggregate((t, i) => t + i);
            sb.AppendLine(indentString + guide);
            sb.AppendLine(indentString + divide);

            foreach (var line in wrapped)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}