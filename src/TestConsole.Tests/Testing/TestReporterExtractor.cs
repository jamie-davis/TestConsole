using FluentAssertions;
using NUnit.Framework;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Testing
{
    [TestFixture]
    public class TestReporterExtractor
    {
        [Test]
        public void IfNoReporterIsSetInSettingsExtractReturnsNotSpecified()
        {
            //Arrange
            var file = new [] {"not right"};

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.Result.Should().Be(ExtractedReporterResult.NotSpecified);
        }

        [Test]
        public void IfNoReporterIsSetInSettingsReporterNameIsNull()
        {
            //Arrange
            var file = new [] {"not right"};

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            Assert.That(result.ReporterName, Is.Null);
        }

        [Test]
        public void IfReporterIsSetInSettingsExtractReturnsSelected()
        {
            //Arrange
            var file = new [] {"reporter=winmerge"};

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.Result.Should().Be(ExtractedReporterResult.Selected);
        }

        [Test]
        public void IfAReporterIsSetInSettingsReporterNameIsSet()
        {
            //Arrange
            var file = new [] {"reporter=winmerge"};

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            Assert.That(result.ReporterName, Is.EqualTo("winmerge"));
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
            var result = ReporterExtractor.Extract(file);

            //Assert
            Assert.That(result.ReporterName, Is.EqualTo("winmerge"));
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
            var result = ReporterExtractor.Extract(file);

            //Assert
            Assert.That(result.ReporterName, Is.EqualTo("kompare"));
        }

        [Test]
        public void IfCustomReporterIsSpecifiedExtractReturnsCustom()
        {
            //Arrange
            var file = new []
            {
                "custom=somefile",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.Result.Should().Be(ExtractedReporterResult.Custom);
        }

        [Test]
        public void IfCustomReporterIsSpecifiedExtractReturnsCustomPath()
        {
            //Arrange
            var file = new []
            {
                "custom=somefile",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.CustomFileName.Should().Be("somefile");
        }

        [Test]
        public void IfCustomReporterWithoutArgsIsSpecifiedExtractReturnsNullArgs()
        {
            //Arrange
            var file = new []
            {
                "custom=somefile",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.CustomArgTemplate.Should().BeNull();
        }

        [Test]
        public void IfCustomReporterWithArgsIsSpecifiedExtractReturnsArgs()
        {
            //Arrange
            var file = new []
            {
                "custom=somefile",
                "template=template",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.CustomArgTemplate.Should().Be("template");
        }

        [Test]
        public void ExtractReturnsBuiltInIfItsFirstInFile()
        {
            //Arrange
            var file = new []
            {
                "reporter=kompare",
                "custom=somefile",
                "template=template",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.ReporterName.Should().Be("kompare");
        }

        [Test]
        public void ExtractReturnsCustomIfItsFirstInFile()
        {
            //Arrange
            var file = new []
            {
                "custom=somefile",
                "reporter=kompare",
                "custom=notthis",
                "custom=orthis",
                "template=template",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.CustomFileName.Should().Be("somefile");
        }

        [Test]
        public void ExtractReturnsCustomArgsEvenIfSplit()
        {
            //Arrange
            var file = new []
            {
                "custom=somefile",
                "reporter=kompare",
                "custom=notthis",
                "custom=orthis",
                "template=template",
                "template=notthis",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.CustomArgTemplate.Should().Be("template");
        }

        [Test]
        public void CustomArgsCanPrecedeFile()
        {
            //Arrange
            var file = new []
            {
                "template=template",
                "custom=somefile",
                "reporter=kompare",
                "custom=notthis",
                "custom=orthis",
                "template=notthis",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.CustomArgTemplate.Should().Be("template");
        }

        [Test]
        public void CustomArgsDoNotForceCustomIfReporterIsFoundFirst()
        {
            //Arrange
            var file = new []
            {
                "template=notused",
                "reporter=thisone",
                "custom=toolate",
                "custom=notthis",
                "custom=orthis",
                "template=notthis",
            };

            //Act
            var result = ReporterExtractor.Extract(file);

            //Assert
            result.ReporterName.Should().Be("thisone");
        }
    }
}
