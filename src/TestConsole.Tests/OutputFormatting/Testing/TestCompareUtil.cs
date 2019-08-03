using System.Linq;
using NUnit.Framework;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Testing
{
    [TestFixture]
    public class TestCompareUtil
    {
        [ApprovalsReporter("testtool")]
        public class TestToolReporter : IApprovalsReporter
        {
            public string FileName => "testtool";
            public string Arguments => "$1 $2";
        }

        [Test]
        public void ReportersCanBeAdded()
        {
            //Arrange
            CompareUtil.RegisterReporters(GetType().Assembly);
            
            //Act/Assert
            var output = new Output();
            output.FormatTable(CompareUtil.ListReporters().Select(t => new {t.ReporterName, t.ReporterType}));
            output.Report.Verify();
        }

        [Test]
        public void ReportersCanBeAddedMultipleTimesWithoutProblems()
        {
            //Act
            CompareUtil.RegisterReporters(GetType().Assembly);
            CompareUtil.RegisterReporters(GetType().Assembly);
            CompareUtil.RegisterReporters(GetType().Assembly);
            CompareUtil.RegisterReporters(GetType().Assembly);
            CompareUtil.RegisterReporters(GetType().Assembly);

            //Assert
            var output = new Output();
            output.FormatTable(CompareUtil.ListReporters().Select(t => new {t.ReporterName, t.ReporterType}));
            output.Report.Verify();
        }
    }
}
