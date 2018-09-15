using NUnit.Framework;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Testing
{
    [TestFixture]
    public class TestReporterExtractor
    {
        [Test]
        public void IfNoReporterIsSetInSettingsExtractReturnsFalse()
        {
            //Arrange
            var file = new [] {"not right"};

            //Act
            var result = ReporterExtractor.Extract(file, out var reporter);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IfNoReporterIsSetInSettingsReporterNameIsNull()
        {
            //Arrange
            var file = new [] {"not right"};

            //Act
            var result = ReporterExtractor.Extract(file, out var reporter);

            //Assert
            Assert.That(reporter, Is.Null);
        }

        [Test]
        public void IfReporterIsSetInSettingsExtractReturnsTrue()
        {
            //Arrange
            var file = new [] {"reporter=winmerge"};

            //Act
            var result = ReporterExtractor.Extract(file, out var reporter);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IfAReporterIsSetInSettingsReporterNameIsSet()
        {
            //Arrange
            var file = new [] {"reporter=winmerge"};

            //Act
            var result = ReporterExtractor.Extract(file, out var reporter);

            //Assert
            Assert.That(reporter, Is.EqualTo("winmerge"));
        }

        [Test]
        public void NonReporterSettingsAreIgnored()
        {
            //Arrange
            var file = new []
            {
                "other=something",
                "reporter=winmerge",
            };

            //Act
            var result = ReporterExtractor.Extract(file, out var reporter);

            //Assert
            Assert.That(reporter, Is.EqualTo("winmerge"));
        }

        [Test]
        public void OnlyTheFirstReporterSettingIsUsed()
        {
            //Arrange
            var file = new []
            {
                "reporter=kompare",
                "other=something",
                "reporter=winmerge",
            };

            //Act
            var result = ReporterExtractor.Extract(file, out var reporter);

            //Assert
            Assert.That(reporter, Is.EqualTo("kompare"));
        }
    }
}
