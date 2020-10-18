using System.Linq;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    public class TestMinReportWidthCalculator
    {
        private static string[] _numbers = {"Zero", "One", "Two", "Three"};

        [Test]
        public void MinWidthIsMinimumIdealWidthOfEachColumn()
        {
            var rep = Enumerable.Range(0, 3)
                .Select(i => new {Number = i, String = _numbers[i]});

            Assert.That(MinReportWidthCalculator.Calculate(rep, 1, true), Is.EqualTo(13));
        }
    }
}
