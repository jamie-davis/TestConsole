using System.Linq;

namespace TestConsole.Tests.OutputFormatting.UnitTestutilities
{
    internal static class RulerFormatter
    {
        public static string MakeRuler(int width)
        {
            return string.Join(string.Empty, Enumerable.Range(0, (width/10) + 1).Select(i => "----+----|"))
                .Substring(0, width);
        }
    }
}
