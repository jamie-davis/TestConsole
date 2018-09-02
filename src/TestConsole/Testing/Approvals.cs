using System.Text;

namespace TestConsoleLib.Testing
{
    public static class Approvals
    {
        public static void Verify(string text)
        {
            text.Verify();
        }

        public static void Verify(StringBuilder text)
        {
            Verify(text.ToString());
        }
    }
}