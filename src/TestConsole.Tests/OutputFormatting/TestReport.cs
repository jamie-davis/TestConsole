﻿using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using TestConsole.OutputFormatting;
using TestConsole.Tests.TestingUtilities;
using TestConsoleLib;

namespace TestConsole.Tests.OutputFormatting
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestReport
    {
        private Output _adapter;
        private OutputBuffer _buffer;

        [SetUp]
        public void SetUp()
        {
            _buffer = new OutputBuffer();
            _buffer.BufferWidth = 80;

            _adapter = new Output(_buffer);
        }

        [Test]
        public void AReportIsGenerated()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign()));

            //Assert
            Assert.That(report.Columns.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ReportIsDisplayedWithDefaultOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data
                .Select(n => new {TestValue = string.Format("Test value {0}", n)})
                .AsReport(rep => rep.AddColumn(n => n.TestValue,
                                               col => col.RightAlign()));
            
            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_buffer.GetBuffer());
        }

        [Test]
        public void TheOmitHeadingsOptionSetsTheReportOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .OmitHeadings());

            //Assert
            Assert.That(report.Options, Is.EqualTo(ReportFormattingOptions.OmitHeadings));
        }

        [Test]
        public void TheOmitHeadingsOptionIsApplied()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .OmitHeadings());

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_buffer.GetBuffer());
        }

        [Test]
        public void TheStretchColumnsOptionSetsTheReportOptions()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .StretchColumns());

            //Assert
            Assert.That(report.Options, Is.EqualTo(ReportFormattingOptions.StretchColumns));
        }

        [Test]
        public void TheStretchColumnsOptionIsApplied()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .StretchColumns());

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_buffer.GetBuffer());
        }

        [Test]
        public void AllOptionsCanBeSetAtOnce()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.StretchColumns()
                                                 .OmitHeadings()
                                                 .AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 );

            //Assert
            var expected = ReportFormattingOptions.StretchColumns | ReportFormattingOptions.OmitHeadings;
            Assert.That(report.Options, Is.EqualTo(expected));
        }

        [Test]
        public void ChildReportsCanBeAdded()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);

            //Act
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.Heading("Test values"))
                                                 .AddChild(n => Enumerable.Range(1, 4),
                                                           p => p.AddColumn(i => i, c => c.Heading("Nested Number")))
                );

            //Assert
            _adapter.FormatTable(report);
            Approvals.Verify(_buffer.GetBuffer());
        }

        [Test]
        public void TheReportCanBeIndented()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var indentReport = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .Indent(4)
                                                 .StretchColumns());
            var normalReport = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .StretchColumns());

            //Act
            _adapter.FormatTable(indentReport);
            _adapter.FormatTable(normalReport);

            //Assert
            Approvals.Verify(_buffer.GetBuffer());
        }

        [Test]
        public void ReportTitleIsPrinted()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .Indent(4)
                                                 .StretchColumns()
                                                 .Title("Test Report"));

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_buffer.GetBuffer());
        }

        [Test]
        public void ReportTitleIsWrapped()
        {
            //Arrange
            var data = Enumerable.Range(0, 4);
            var report = data.AsReport(rep => rep.AddColumn(n => string.Format("Test value {0}", n),
                                                            col => col.RightAlign())
                                                 .Indent(4)
                                                 .StretchColumns()
                                                 .Title("ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ ABCDEF GHIJKLM NOPQRST UVWXYZ"));

            //Act
            _adapter.FormatTable(report);

            //Assert
            Approvals.Verify(_buffer.GetBuffer());
        }
    }
}
