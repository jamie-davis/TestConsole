using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TestConsoleLib;
using TestConsoleLib.Testing;
using TestConsoleLib.Utilities;

namespace TestConsole.Tests.Utilities
{
    [TestFixture]
    public class TestNumericTypeHelper
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
        public void NumericTypesAreIdentified()
        {
            //Arrange
            var output = new Output();
            
            //Act
            output.WrapLine("Numeric types");
            output.WriteLine();
            output.FormatTable(NumericTypes.Select(t => new { Type = t.Name, IsNumeric = NumericTypeHelper.IsNumeric(t) }));
            output.WriteLine();
            output.WriteLine();
            output.WriteLine();
            output.WrapLine("Non-Numeric types");
            output.WriteLine();
            output.FormatTable(NonNumericTypes.Select(t => new { Type = t.Name, IsNumeric = NumericTypeHelper.IsNumeric(t) }));

            //Assert
            output.Report.Verify();
        }
    }
}
