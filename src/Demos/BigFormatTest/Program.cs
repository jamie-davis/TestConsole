using System;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace BigFormatTest
{
    class Program : ConsoleApplication
    {
        static void Main(string[] args)
        {
            Toolkit.Execute<Program>(args);
        }

        #region Overrides of ConsoleApplicationBase

        protected override void Initialise()
        {
            HelpOption<Options>(o => o.Help);
            base.Initialise();
        }

        #endregion
    }

    [Command]
    public class Options
    {
        [Option("help", "h")]
        [Description("Display help text")]
        public bool Help { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console)
        {
            TestLargeFormat.LargeFormatPerformanceTest(console);
        }
    }


}
