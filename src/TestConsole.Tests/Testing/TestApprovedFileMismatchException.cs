using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TestConsoleLib;
using TestConsoleLib.Exceptions;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Testing
{
    [TestFixture]
    public class TestApprovedFileMismatchException
    {
        [Test]
        public void ExceptionMessageIsFormatted()
        {
            //Arrange
            Exception exception = null;
            var approvedText = @"Line 1
Line 2";
            var receivedText = @"Line 1
Line 2 Diff";

            using (var approved = WriteTempFile(approvedText))
            using (var received = WriteTempFile(receivedText))
            {
                //Act
                exception = new ApprovedFileMismatchException(received.TempPath, approved.TempPath);
            }

            //Assert
            exception.Message.Verify();
        }

        [Test]
        public void ExceptionMessageHandlesEmptyReceivedFile()
        {
            //Arrange
            Exception exception = null;
            var approvedText = @"Line 1
Line 2";
            var receivedText = string.Empty;

            using (var approved = WriteTempFile(approvedText))
            using (var received = WriteTempFile(receivedText))
            {
                //Act
                exception = new ApprovedFileMismatchException(received.TempPath, approved.TempPath);
            }

            //Assert
            exception.Message.Verify();
        }

        [Test]
        public void ExceptionMessageHandlesEmptyApprovedFile()
        {
            //Arrange
            Exception exception = null;
            var approvedText  = string.Empty;
            var receivedText= @"Line 1
Line 2";

            using (var approved = WriteTempFile(approvedText))
            using (var received = WriteTempFile(receivedText))
            {
                //Act
                exception = new ApprovedFileMismatchException(received.TempPath, approved.TempPath);
            }

            //Assert
            exception.Message.Verify();
        }

        [Test]
        public void ExceptionMessageHandlesIdenticalData()
        {
            //Arrange
            Exception exception = null;
            var approvedText  = string.Empty;

            using (var approved = WriteTempFile(approvedText))
            using (var received = WriteTempFile(approvedText))
            {
                //Act
                exception = new ApprovedFileMismatchException(received.TempPath, approved.TempPath);
            }

            //Assert
            exception.Message.Verify();
        }

        [Test]
        public void MaximumDifferencesAreReported()
        {
            //Arrange
            Exception exception = null;
            var approvedText = string.Join(Environment.NewLine, Enumerable.Range(1, 100).Select(n => $"Line {n}"));
            var receivedText = string.Join(Environment.NewLine, Enumerable.Range(1, 100).Select(n => $"Line {((n & 1) != 0 ? "X" : n.ToString())}"));

            using (var approved = WriteTempFile(approvedText))
            using (var received = WriteTempFile(receivedText))
            {
                //Act
                exception = new ApprovedFileMismatchException(received.TempPath, approved.TempPath);
            }

            //Assert
            exception.Message.Verify();
        }


        #region Temporary file handling

        private TempFile WriteTempFile(string text)
        {
            return new TempFile(text);
        }

        private class TempFile : IDisposable
        {
            public string TempPath { get; }

            public TempFile(string text)
            {
                TempPath = Path.GetTempFileName();
                try
                {
                    File.WriteAllText(TempPath, text);
                }
                catch
                {
                    Dispose(); //We will not complete construction, don't leave the temp file in place
                    throw;
                }
            }

            #region IDisposable

            public void Dispose()
            {
                File.Delete(TempPath);
            }

            #endregion
        }

        #endregion
    }
}
