using System.Globalization;
using System.Threading;

namespace TestConsole.Tests.TestingUtilities
{
    public static class SetUpTests
    {
        public static void OverrideCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB", true);
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        }
    }
}