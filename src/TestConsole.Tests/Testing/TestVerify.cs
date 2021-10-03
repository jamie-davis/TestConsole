using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TestConsoleLib.Testing;
#pragma warning disable 1998

namespace TestConsole.Tests.Testing
{
    [TestFixture]
    public class TestVerify
    {
        [Test]
        public void CheckSimpleVerifyRuns()
        {
            //Assert
            "Simple verify extension call".Verify();
        }

        [Test]
        public async Task CheckAsyncVerifyRuns()
        {
            //Act/Assert
            "Verify in async call".Verify();
        }

        [Test]
        public async Task CheckAsyncWithAwaitVerifyRuns()
        {
            //Arrange
            await Task.Run(() => { });

            //Act/Assert
            "Verify after await in async call".Verify();
        }

        [Test]
        public async Task CheckVerifyWithEncodingRuns()
        {
            //Assert
            const string verifyText = "Verify with encoding";
            $"\u250C{new string('\u2500', verifyText.Length)}\u2510\n\u2502{verifyText}\u2502\n\u2514{new string('\u2500', verifyText.Length)}\u2518".Verify(Encoding.UTF8);
        }
    }
}
