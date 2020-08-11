using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Testing
{
    [TestFixture]
    public class TestStackInterpreter
    {
        private string ClassPath([CallerFilePath] string callerFilePath = null)
        {
            return callerFilePath;
        }

        private string CallingMethod([CallerMemberName] string callerMemberName = null)
        {
            return callerMemberName;
        }

        [Test]
        public async Task AsyncStackFrameInLamdaDetected()
        {
            //Arrange
            CallerStackFrameInfo frame = null;

            //Act
            await Task.Run(() => frame = StackInterpreter.GetCallerStackFrameInfo(CallingMethod(), ClassPath()));

            //Assert
            frame?.Method?.Should().Be(GetType().GetMethod(CallingMethod()));
        }

        [Test]
        public async Task AsyncStackFrameDetected()
        {
            //Arrange
            CallerStackFrameInfo frame = null;

            //Act
            await Task.Run(() => { });
            frame = StackInterpreter.GetCallerStackFrameInfo(CallingMethod(), ClassPath());

            //Assert
            frame?.Method?.Should().Be(GetType().GetMethod(CallingMethod()));
        }
    }
}