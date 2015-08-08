using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.Internal;
using TestConsole.Tests.OutputFormatting.UnitTestutilities;
using TestConsole.Tests.TestingUtilities;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestConsoleOperationsImpl
    {
        private OutputBuffer _consoleInterface;
        private ConsoleOperationsImpl _adapter;
        private ConsoleOperationsImpl _prefixAdapter;

        [SetUp]
        public void SetUp()
        {
            _consoleInterface = new OutputBuffer();
            _consoleInterface.BufferWidth = 60;

            _adapter = new ConsoleOperationsImpl(_consoleInterface);
            _prefixAdapter = new ConsoleOperationsImpl(_consoleInterface, "prefix: ");
        }

        /// <summary>
        /// Execute a test on both the prefixed and unprefixed adapter
        /// </summary>
        /// <param name="testAction">The "act" part of the test.</param>
        private void Execute(Action<ConsoleOperationsImpl> testAction)
        {
            _consoleInterface.Write("Normal adapter:");
            _consoleInterface.NewLine();
            _consoleInterface.Write("-->");
            _consoleInterface.NewLine();
            testAction(_adapter);
            _consoleInterface.Write("<--");
            _consoleInterface.NewLine();
            _consoleInterface.NewLine();
            _consoleInterface.Write("Prefixed adapter:");
            _consoleInterface.NewLine();
            _consoleInterface.Write("-->");
            _consoleInterface.NewLine();
            testAction(_prefixAdapter);
            _consoleInterface.Write("<--");
        }

        [Test]
        public void LinesAreWrittenToTheConsole()
        {
            Action<ConsoleOperationsImpl> act = adapter =>
                                                         {
                                                             adapter.WriteLine("Console {0}", "output");
                                                             adapter.WriteLine("More output.");
                                                         };

            Execute(act);

            var buffer = _consoleInterface.GetBuffer();
            Console.WriteLine(buffer);
            Approvals.Verify(buffer);
        }

        [Test]
        public void FullWidthTabularDataIsDisplayed()
        {
            _consoleInterface.BufferWidth = _consoleInterface.BufferWidth;

            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.WriteLine(RulerFormatter.MakeRuler(adapter.BufferWidth));
                                                        var data = Enumerable.Range(1, 10)
                                                                             .Select(i => new { Number = i, String = string.Join(" ",
                                                                                                 Enumerable.Repeat("blah", i))
                                                                                     });
                                                        adapter.FormatTable(data);
                                                    };

            Execute(act);
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void ShortWidthTabularDataIsDisplayed()
        {
            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.WriteLine(RulerFormatter.MakeRuler(adapter.BufferWidth));
                                                        var data = Enumerable.Range(1, 10)
                                                                             .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });
                                                        adapter.FormatTable(data);
                                                    };

            Execute(act);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void WrapLinesAreWordWrappedToConsole()
        {
            _consoleInterface.BufferWidth = _consoleInterface.BufferWidth;
            _consoleInterface.Write(RulerFormatter.MakeRuler(_consoleInterface.BufferWidth));

            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.WrapLine("Long console output that should require word wrapping to fit the display width, but remain short of the line end to illustrate WrapLine.");
                                                        adapter.WrapLine("More output, and hopefully more wrapping. Must ensure that there is enough text. Also WrapLine'd.");
                                                        adapter.WrapLine("No wrapping needed.");
                                                    };
            Execute(act);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void SingleLongDataLineIsWrappedCorrectly()
        {
            _consoleInterface.BufferWidth = _consoleInterface.BufferWidth;
            _consoleInterface.Write(RulerFormatter.MakeRuler(_consoleInterface.BufferWidth));

            var text = "Long console output that should require word wrapping to fit the display width, specifically crafted "
                       + "so that it can span at least three lines in the output. This will exercise the wrapping functionality such that "
                       + "any issues with the display width should become obvious.";


            Action<ConsoleOperationsImpl> act = adapter => adapter.WrapLine(text);
            Execute(act);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void StringContainingBracesButNoSubstitutionsIsRenderedCorrectly()
        {
            _consoleInterface.BufferWidth = _consoleInterface.BufferWidth;
            _consoleInterface.Write(RulerFormatter.MakeRuler(_consoleInterface.BufferWidth));

            var text = "{data}";
            
            Action<ConsoleOperationsImpl> act = adapter => adapter.WrapLine(text);
            Execute(act);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void PiecemealWritesAreWordWrappedToConsole()
        {
            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.WriteLine(RulerFormatter.MakeRuler(adapter.BufferWidth));
                                                        adapter.Wrap("Long console output ");
                                                        adapter.Wrap("that should require word wrapping to");
                                                        adapter.Wrap(" fit the display width.");
                                                        adapter.Wrap(" This was ");
                                                        adapter.Wrap("split");
                                                        adapter.Wrap(" into");
                                                        adapter.Wrap(" many small writes");
                                                    };
            Execute(act);

            var buffer = _consoleInterface.GetBuffer();
            Console.WriteLine(buffer);
            Approvals.Verify(buffer);
        }

        [Test]
        public void RecordedOperationsCanBeDisplayedOnTheConsole()
        {
            var recorder = MakeRecording();

            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.Write(recorder);
                                                        adapter.Write("XXX");
                                                    };

            Execute(act);
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void RecordedOperationsStartOnANewLine()
        {
            var recorder = MakeRecording();

            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.Write("XXX");
                                                        adapter.Write(recorder);
                                                    };

            Execute(act);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void RecordedOperationsCanBeDisplayedOnTheConsoleUsingWriteLine()
        {
            var recorder = MakeRecording();

            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.WriteLine(recorder);
                                                        adapter.WriteLine("XXX");
                                                    };

            Execute(act);
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void RecordedOperationsDisplayedWithWriteLineStartOnANewLine()
        {
            var recorder = MakeRecording();

            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.Write("XXX");
                                                        adapter.WriteLine(recorder);
                                                        adapter.WriteLine("XXX");
                                                    };

            Execute(act);
            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test]
        public void ReportFormattedTableIsDisplayed()
        {
            Action<ConsoleOperationsImpl> act = adapter =>
                                                    {
                                                        adapter.WriteLine(RulerFormatter.MakeRuler(adapter.BufferWidth));
                                                        var data = Enumerable.Range(1, 10)
                                                                             .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) })
                                                                             .AsReport(p => p.AddColumn(t => t.String.Length < 10 ? t.String.PadRight(10, '*') : t.String.Substring(0, 10), c => c.Heading("First 10"))
                                                                                             .AddColumn(t => t.Number / 2, c => c.Heading("Halves"))
                                                                                             .StretchColumns());
                                                        adapter.FormatTable(data);
                                                    };

            Execute(act);

            Approvals.Verify(_consoleInterface.GetBuffer());
        }

        [Test, ExpectedException]
        public void BadCodeInReportFormattingThrowsAnException()
        {
            _adapter.WriteLine(RulerFormatter.MakeRuler(_adapter.BufferWidth));
            var data = Enumerable.Range(1, 10)
                                 .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) })
                                 .AsReport(p => p.AddColumn(t => t.String.Substring(0, 10), c => c.Heading("Bad"))
                                                 .AddColumn(t => t.Number / 2, c => c.Heading("Halves")));
            _adapter.FormatTable(data);
            Console.WriteLine(_consoleInterface.GetBuffer()); //shouldn't reach here, so any output may be informative
        }


        [Test]
        public void FormatObjectProducesAReportStyleDumpOfTheObject()
        {
            //Act
            _adapter.FormatObject(Tuple.Create(1, "AAA", "BBB", Tuple.Create(2, "aaaa", "bbb")));

            //Assert
            Approvals.Verify(_consoleInterface.GetBuffer());
        }


        private static RecordingConsoleAdapter MakeRecording()
        {
            var recorder = new RecordingConsoleAdapter();
            recorder.Write("Write.");
            recorder.WriteLine("WriteLine.");
            recorder.Wrap("Some wrapped text ");
            recorder.Wrap("added in chunks, followed by ");
            recorder.WrapLine("a final wrap line.");
            var data = Enumerable.Range(1, 10)
                                 .Select(i => new { Number = i, String = string.Join(" ", Enumerable.Repeat("blah", i)) });
            recorder.FormatTable(data);
            recorder.Write("<--- END");
            return recorder;
        }
    }
}