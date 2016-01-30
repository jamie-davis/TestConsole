using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using TestConsole.Tests.TestingUtilities;
using TestConsoleLib;
using TestConsoleLib.CallRecording;

namespace TestConsole.Tests.CallRecording
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCallRecorder
    {
        private Output _output;

        #region Test Types

        public interface ITest
        {
            void NoParamCall();
            void ValueParamCall(int n);
            void ObjectParamCall(TestClass param);
            int ValueReturn();
            TestClass ObjectReturn();
            void ValueOutParameter(out int n);
            int ObjectOutParameter(out TestClass testClass);
        }

        class TestImpl : ITest
        {
            private bool _throw;
            public bool NoParamCalled { get; set; }

            public void NoParamCall()
            {
                NoParamCalled = true;

                if (_throw)
                    throw new ApplicationException("Exception");
            }

            public void ValueParamCall(int n)
            {
                ValueParam = n;
            }

            public void ObjectParamCall(TestClass param)
            {
                ObjectParam = param;
            }

            public int ValueReturn()
            {
                return 2015;
            }

            public TestClass ObjectReturn()
            {
                return new TestClass("test string", 2015);
            }

            public void ValueOutParameter(out int n)
            {
                n = 2015;
            }

            public int ObjectOutParameter(out TestClass testClass)
            {
                testClass = new TestClass("out", 2015);
                return 100;
            }

            public int ValueParam { get; set; }
            public TestClass ObjectParam { get; set; }

            public TestImpl()
            {
                NoParamCalled = false;
            }

            public void Throw()
            {
                _throw = true;
            }
        }

        public class TestClass
        {
            public string StringVar { get; set; }
            public int IntVar { get; set; }

            public TestClass(string stringVar, int intVar)
            {
                StringVar = stringVar;
                IntVar = intVar;
            }
        }

        public interface ITypeTester<T>
        {
            void Param(T param);
            T Return();
            void OutParam(out T param);
            void RefParam(ref T param);
        }

        class TypeTester<T> : ITypeTester<T>
        {
            public T SampleValue;

            public void Param(T param)
            {
            }

            public T Return()
            {
                return SampleValue;
            }

            public void OutParam(out T param)
            {
                param = SampleValue;
            }

            public void RefParam(ref T param)
            {
                param = SampleValue;
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _output = new Output();
        }

        [Test]
        public void InterfaceCallsAreTracked()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);

            //Act
            recorder.NoParamCall();

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void InterfaceCallsAreMadeByWrapper()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);

            //Act
            recorder.NoParamCall();

            //Assert
            Assert.That(testImpl.NoParamCalled, Is.True);
        }

        [Test]
        public void ValueParametersAreTracked()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);

            //Act
            recorder.ValueParamCall(2015);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ObjectParametersAreTracked()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);

            //Act
            recorder.ObjectParamCall(new TestClass("string", 500));

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ValueParametersArePassedByWrapper()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);

            //Act
            recorder.ValueParamCall(2015);

            //Assert
            Assert.That(testImpl.ValueParam, Is.EqualTo(2015));
        }

        [Test]
        public void ValueReturnedIsReported()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);

            //Act
            recorder.ValueReturn();

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ObjectReturnedIsReported()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);

            //Act
            recorder.ObjectReturn();

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void OutValueReturnedIsReported()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);
            var n = 5000;

            //Act
            recorder.ValueOutParameter(out n);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void OutObjectReturnedIsReported()
        {
            //Arrange
            var testImpl = new TestImpl();
            var recorder = CallRecorder.Generate((ITest) testImpl, _output);
            TestClass test;

            //Act
            recorder.ObjectOutParameter(out test);

            //Assert
            Approvals.Verify(_output.Report);
        }

        private static class DataTypeTester
        {
            public static DataTypeTester<T> Create<T>(T value)
            {
                return new DataTypeTester<T>(value);
            }

            public static DataTypeTester<T> Create<T>(T value, T initialiser)
            {
                return new DataTypeTester<T>(value, initialiser);
            }
        }

        private class DataTypeTester<T> : IDataTypeTester
        {
            private readonly T _value;
            private readonly T _initialiser;

            public DataTypeTester(T value)
            {
                _value = value;
                _initialiser = default(T);
            }

            public DataTypeTester(T value, T initialiser)
            {
                _value = value;
                _initialiser = initialiser;
            }

            public void Run(Output output)
            {
                var tester = new TypeTester<T>();
                tester.SampleValue = _value;
                var wrapper = CallRecorder.Generate((ITypeTester<T>) tester, output);

                var name = string.Format("Running test on {0}", typeof (T));
                var line = new string('-', name.Length);
                output.WrapLine(line);
                output.WrapLine(name);
                output.WrapLine(line);
                wrapper.Param(_value);
                output.WriteLine();
                wrapper.Return();
                output.WriteLine();
                
                T outValue = _initialiser, outValue2 = _initialiser;
                wrapper.OutParam(out outValue);
                output.WriteLine();
                wrapper.RefParam(ref outValue2);
                output.WriteLine();
            }
        }

        private interface IDataTypeTester
        {
            void Run(Output output);
        }

        private static readonly IDataTypeTester[] DataTypeTestCases = 
        {
            DataTypeTester.Create((decimal?)150.5, -200m),
            DataTypeTester.Create(decimal.MaxValue, 150.5m),
            DataTypeTester.Create(DBNull.Value),
            DataTypeTester.Create(true),
            DataTypeTester.Create((bool?)true, false),
            DataTypeTester.Create(new [] {"a", "b", "c"}, new [] {"1", "2", "3"}),
            DataTypeTester.Create(DateTime.MaxValue, DateTime.MinValue),
            DataTypeTester.Create(DateTime.MaxValue, new DateTime?()),
            DataTypeTester.Create(float.MaxValue, (float)50.5),
            DataTypeTester.Create(float.MaxValue, new float?()),
            DataTypeTester.Create(double.MaxValue, 50.5),
            DataTypeTester.Create(double.MaxValue, new double?()),
            DataTypeTester.Create(byte.MaxValue),
            DataTypeTester.Create(sbyte.MaxValue),
            DataTypeTester.Create('x', 'y'),
            DataTypeTester.Create(short.MaxValue),
            DataTypeTester.Create(ushort.MaxValue),
            DataTypeTester.Create(int.MaxValue),
            DataTypeTester.Create(uint.MaxValue),
            DataTypeTester.Create(long.MaxValue),
            DataTypeTester.Create(ulong.MaxValue),
            DataTypeTester.Create("text")
        };

        [Test]
        public void CheckAllDataTypesAreValid()
        {            
            //Act
            foreach (var testCase in DataTypeTestCases)
            {
                testCase.Run(_output);
            }

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ExceptionsAreReported()
        {
            //Arrange
            var testImpl = new TestImpl();
            testImpl.Throw();
            var recorder = CallRecorder.Generate((ITest)testImpl, _output);

            //Act
            try
            {
                recorder.NoParamCall();
            }
            catch (Exception e)
            {
                // ignored
                _output.WrapLine("Kaboom {0}", e.Message);
            }

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void ExceptionsAreRethrown()
        {
            //Arrange
            var testImpl = new TestImpl();
            testImpl.Throw();
            var recorder = CallRecorder.Generate((ITest)testImpl, _output);

            //Act
            recorder.NoParamCall();
        }
    }

}
