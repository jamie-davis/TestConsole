using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using TestConsoleLib;
using TestConsoleLib.Testing;
using ReportFormattingOptions = TestConsole.OutputFormatting.ReportFormattingOptions;

// ReSharper disable UnusedType.Global

namespace BigFormatTest
{
    public static class TestLargeFormat
    {
        private static (int Value, string Expected)[] _tests =
        {
            (0, "zero"),
            (9, "nine"),
            (99, "ninety nine"),
            (100, "one hundred"),
            (900, "nine hundred"),
            (901, "nine hundred and one"),
            (1901, "one thousand, nine hundred and one"),
            (132456, "one hundred and thirty two thousand, four hundred and fifty six"),
            (77000000, "seventy seven million"),
            (2000000000, "two billion"),
            (int.MaxValue, "two billion, one hundred and forty seven million, four hundred and eighty three thousand, six hundred and forty seven"),
            (int.MinValue, "minus two billion, one hundred and forty seven million, four hundred and eighty three thousand, six hundred and forty eight"),
        };
 
        public static void LargeFormatPerformanceTest(IConsoleAdapter console, string pathToSaveReport)
        {
            //Arrange
            console.FormatTable(_tests.Select(t =>
            {
                var actual = NumberAsText(t.Value);
                return new {t.Value, t.Expected, Actual = actual == t.Expected ? actual.Green() : actual.Red()};
            }));

            var rows = Enumerable.Range(1, 200000).Select(n =>
            {
                var n2 = n * 450;
                var n3 = n * 1311;
                n *= 127;
                return new
                {
                    BaseValue = n, Text = NumberAsText(n), Length = NumberAsText(n).Length, 
                    Value2 = n2, Text2 = NumberAsText(n2),
                    Value3 = n3, Text3 = NumberAsText(n3),
                };
            }).ToList();

            var output = new Output();
            
            var sw = new Stopwatch();
            sw.Start();

            //Act
            output.FormatTable(rows, ReportFormattingOptions.UnlimitedBuffer);
            sw.Stop();

            //Report
            console.WrapLine($"Report time: {sw.Elapsed.TotalSeconds.ToString().White()} seconds".Cyan());
            var report = output.Report;
            console.WrapLine($"Report length: {report.Length.ToString().White()} characters".Cyan());

            if (!string.IsNullOrWhiteSpace(pathToSaveReport))
                File.WriteAllText(pathToSaveReport, report);
        }

        private static (int Limit, string Desc)[] _descriptions =
        {
            (100, "hundred"),
            (1000, "thousand"),
            (1000000, "million"),
            (1000000000, "billion"),
        };

        private static string[] _tens =
        {
            "twenty",
            "thirty",
            "forty",
            "fifty",
            "sixty",
            "seventy",
            "eighty",
            "ninety",
        };

        private static string[] _lowNumbers =
        {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "ten",
            "eleven",
            "twelve",
            "thirteen",
            "fourteen",
            "fifteen",
            "sixteen",
            "seventeen",
            "eighteen",
            "nineteen",
        };

        private static string NumberAsText(int n)
        {
            var sb = new StringBuilder();

            if (n < 0)
                sb.Append("minus ");

            foreach (var description in _descriptions.Reverse())
            {
                if (n >= description.Limit || n < (0 - description.Limit))
                {
                    var remainder = n % description.Limit;
                    var number = Math.Abs((n - remainder) / description.Limit);
                    n = remainder;
                    sb.Append($"{NumberAsText(number)} {description.Desc}");
                    if (n > 100 || n < -100)
                        sb.Append(",");
                    if (n != 0)
                        sb.Append(" ");
                }
            }

            if (Math.Abs(n % 100) > 0)
            {
                if (sb.Length > 0)
                    sb.Append("and ");
                sb.Append(UnitsAsText(Math.Abs(n)));
            }

            if (sb.Length == 0)
                return "zero";

            return sb.ToString();
        }

        private static string UnitsAsText(int n)
        {
            var tens = (n % 100);
            var sb = new StringBuilder();

            if (tens > 0 && tens < 20)
            {
                sb.Append(_lowNumbers[tens - 1]);
            }
            else if (tens > 0)
            {
                var units = n % 10;
                tens -= units;
                if (tens > 0 && units > 0)
                    sb.Append($"{_tens[(tens/10) - 2]} {_lowNumbers[units - 1]}");
                else if (tens > 0)
                    sb.Append($"{_tens[(tens/10) - 2]}");
                else
                    sb.Append(_lowNumbers[units - 1]);
            }
            return sb.ToString();
        }
    }
}