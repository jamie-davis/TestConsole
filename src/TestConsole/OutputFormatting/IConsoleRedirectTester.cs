namespace TestConsole.OutputFormatting
{
    internal interface IConsoleRedirectTester
    {
        bool IsOutputRedirected();
        bool IsErrorRedirected();
    }
}