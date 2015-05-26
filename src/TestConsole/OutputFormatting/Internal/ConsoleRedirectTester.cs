namespace TestConsole.OutputFormatting.Internal
{
    internal class ConsoleRedirectTester : IConsoleRedirectTester
    {
        private bool _consoleRedirectionTested;
        private bool _outRedirected;
        private bool _errorRedirected;

        /// <summary>
        /// Determine whether the console's output stream is redirected.
        /// </summary>
        public bool IsOutputRedirected()
        {
            return false;
        }

        /// <summary>
        /// Determine whether the console's error stream is redirected.
        /// </summary>
        public bool IsErrorRedirected()
        {
            return false;
        }
    }
}