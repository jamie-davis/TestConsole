using System;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsole.Tests.TestingUtilities;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestColumnFormat
    {
        private readonly static Type[] NumericTypes = 
        {
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(float),
            typeof(double)
        };

        private readonly static Type[] NonNumericTypes = 
        {
            typeof(string),
            typeof(char),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(bool),
            typeof(object),
        };

        [Test]
        public void NumericTypesAreRightAligned()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Numeric data types:");
            foreach (var numericType in NumericTypes)
            {
                sb.AppendFormat("    Alignment = {0}", new ColumnFormat("test", type: numericType).Alignment);
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Non-numeric data types:");
            foreach (var nonNumericType in NonNumericTypes)
            {
                sb.AppendFormat("    Alignment = {0}", new ColumnFormat("test", type: nonNumericType).Alignment);
                sb.AppendLine();
            }

            Console.WriteLine(sb);
            Approvals.Verify(sb.ToString());
        }

        [Test]
        public void DefaultColumnRequiresWidthCalculatedFromData()
        {
            //Arrange
            var format = new ColumnFormat("test", typeof(int));

            //Act
            var result = format.DetermineWidthFromData();

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void FixedWidthColumnDoesNotRequireWidthCalculation()
        {
            //Arrange
            var format = new ColumnFormat("test", typeof(int));

            //Act
            format.FixedWidth = 6;
            var result = format.DetermineWidthFromData();

            //Assert
            Assert.That(result, Is.False);
        }
    }
}