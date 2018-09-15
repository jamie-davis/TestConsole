using System.Linq;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    public class TestTextStats
    {
        [Test]
        public void MaximumLengthIsMeasured()
        {
            var stats = new TextStats();
            ProcessNumberStrings(stats);

            Assert.That((object) stats.MaxWidth, Is.EqualTo(5));
        }

        [Test]
        public void MinimumLengthIsMeasured()
        {
            var stats = new TextStats();
            ProcessNumberStrings(stats);

            Assert.That((object) stats.MinWidth, Is.EqualTo(3));
        }

        [Test]
        public void MinimumLengthCanBeZero()
        {
            var stats = new TextStats();
            stats.Add(string.Empty);
            ProcessNumberStrings(stats);

            Assert.That((object) stats.MinWidth, Is.EqualTo(0));
        }

        [Test]
        public void AverageLengthIsMeasured()
        {
            var stats = new TextStats();

            stats.Add(new string('*',100));
            stats.Add(new string('*',50));

            Assert.That((object) stats.AvgWidth, Is.EqualTo(75.0));
        }

        [Test]
        public void LengthTalliesAreMaintained()
        {
            var stats = new TextStats();

            ProcessNumberStrings(stats);

            var tallies = stats.LengthTallies.Select<TextStats.LengthTally, string>(t => string.Format("[{0} = {1}]", t.Length, t.Count)).Aggregate((t, i) => t + i);

            Assert.That(tallies, Is.EqualTo("[3 = 4][5 = 3][4 = 3]"));
        }

        private static void ProcessNumberStrings(TextStats stats)
        {
            var values = new[] {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"};
            foreach (var value in values)
            {
                stats.Add(value);
            }
        }
    }
}
