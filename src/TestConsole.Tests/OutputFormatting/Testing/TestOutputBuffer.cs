using NUnit.Framework;
using TestConsole.OutputFormatting;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Testing
{
    [TestFixture]
    public class TestOutputBuffer
    {
        private OutputBuffer _console;

        [SetUp]
        public void SetUp()
        {
            _console = new OutputBuffer();
        }

        [Test]
        public void TheCursorPositionAdvancesWhenTextIsOutput()
        {
            _console.Write("text");
            Assert.That(_console.CursorLeft, Is.EqualTo(4));
        }

        [Test]
        public void TextWiderThanTheBufferFlowsToNextLine()
        {
            var longLine = new string('X', _console.BufferWidth + 10);
            _console.Write(longLine);
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void TextAsWideAsTheBufferFlowsCreatesNewLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void TextAsWideAsTheBufferMovesCursorDownOneLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Assert.That(_console.CursorTop, Is.EqualTo(1));
        }

        [Test]
        public void TextAsWideAsTheBufferMovesCursorToStartOfNewLine()
        {
            var longLine = new string('X', _console.BufferWidth);
            _console.Write(longLine);
            Assert.That(_console.CursorLeft, Is.EqualTo(0));
        }

        [Test]
        public void InterfaceIsInValidStateAfterWidthChange()
        {
            _console.BufferWidth = 132;
            _console.Write("text text text");
            Assert.That(_console.CursorLeft, Is.EqualTo(14));
        }
    }
}
