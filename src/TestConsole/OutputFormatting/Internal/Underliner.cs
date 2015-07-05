using System.Text;

namespace TestConsole.OutputFormatting.Internal
{
    internal static class Underliner
    {
        /// <summary>
        /// Generate an underline for the supplied text
        /// </summary>
        /// <param name="line">The text to underline</param>
        /// <returns>The string underlining the provided text</returns>
        public static string Generate(string line, string underline = null)
        {
            var underlineString = underline ?? "-";
            var underlineLength = underlineString.Length;
            var builder = new StringBuilder();
            var widthToGo = line.Length;

            while (widthToGo > underlineLength)
            {
                builder.Append(underlineString);
                widthToGo -= underlineLength;
            }

            if (widthToGo > 0)
                builder.Append(underlineString.Substring(0, widthToGo));

            return builder.ToString();
        }
    }
}