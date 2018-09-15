using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    public class TestUnderliner
    {
        private Output _adapter;

        [SetUp]
        public void SetUp()
        {
            _adapter = new Output();
        }

        [Test]
        public void UnderlinerGeneratesDashesByDefault()
        {
            //Arrange
            var heading = "Test heading";
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading));

            //Assert
            Approvals.Verify(_adapter.Report);
        }

        [Test]
        public void UnderlinerUsesProvidedUnderlineString()
        {
            //Arrange
            var heading = "Test heading";
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading, "*"));

            //Assert
            Approvals.Verify(_adapter.Report);
        }

        [Test]
        public void UnderlinerCanUseMultiCharacterUnderlineString()
        {
            //Arrange
            var heading = "Test heading";
            _adapter.WrapLine(heading);

            //Act
            _adapter.WrapLine(Underliner.Generate(heading, "*-!^?"));

            //Assert
            Approvals.Verify(_adapter.Report);
        }
    }
}
