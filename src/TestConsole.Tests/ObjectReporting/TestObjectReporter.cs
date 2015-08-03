﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using TestConsole.Tests.TestingUtilities;
using TestConsoleLib;
using TestConsoleLib.ObjectReporting;
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable ClassWithVirtualMembersNeverInherited.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable RedundantArgumentDefaultValue

namespace TestConsole.Tests.ObjectReporting
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestObjectReporter
    {
        private Output _output;

        #region Types for test

        class SimpleType
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public DateTime DateValue { get; set; }

            public SimpleType(string stringValue, int intValue, string dateValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                DateValue = DateTime.Parse(dateValue);
            }
        }

        class TypeWithChild
        {
            public SimpleType Child { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public DateTime DateValue { get; set; }

            public TypeWithChild(string stringValue, int intValue, string dateValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                DateValue = DateTime.Parse(dateValue);

                Child = new SimpleType("Child." + stringValue, intValue * 2, (DateValue + new TimeSpan(12, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        class TypeWithChildCollection
        {
            public List<SimpleType> Children { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public DateTime DateValue { get; set; }

            public TypeWithChildCollection(string stringValue, int intValue, string dateValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                DateValue = DateTime.Parse(dateValue);

                Children = Enumerable.Range(0,5)
                    .Select(n => new SimpleType("Child." + n.ToString(), n, (DateValue + new TimeSpan(n, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss")))
                    .ToList();
            }
        }

        class TypeWithChildOCollection
        {
            public ObservableCollection<SimpleType> Children { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public DateTime DateValue { get; set; }

            public TypeWithChildOCollection(string stringValue, int intValue, string dateValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                DateValue = DateTime.Parse(dateValue);

                var childValues = Enumerable.Range(0, 5)
                    .Select(n => new SimpleType("Child." + n.ToString(), 
                        n,
                        (DateValue + new TimeSpan(n, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss")));
                Children = new ObservableCollection<SimpleType>(childValues);
            }
        }

        class DerivedCollection : ObservableCollection<SimpleType>
        {
            public DerivedCollection(IEnumerable<SimpleType> childValues) : base(childValues)
            {
            }
        }

        class TypeWithChildDerivedCollection
        {

            public DerivedCollection Children { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public DateTime DateValue { get; set; }

            public TypeWithChildDerivedCollection(string stringValue, int intValue, string dateValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                DateValue = DateTime.Parse(dateValue);

                var childValues = Enumerable.Range(0, 5)
                    .Select(n => new SimpleType("Child." + n.ToString(), 
                        n,
                        (DateValue + new TimeSpan(n, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss")));
                Children = new DerivedCollection(childValues);
            }
        }

        class TypeWithValueCollection
        {
            public ObservableCollection<int> Children { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public DateTime DateValue { get; set; }

            public TypeWithValueCollection(string stringValue, int intValue, string dateValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                DateValue = DateTime.Parse(dateValue);

                var childValues = Enumerable.Range(0, 5);
                Children = new ObservableCollection<int>(childValues);
            }
        }

        class NestedTypeWIthChildren
        {
            public List<TypeWithValueCollection> Children { get; set; }
            public string Text { get; set; }

            public NestedTypeWIthChildren(string text)
            {
                Text = text;
                Children = Enumerable.Range(0, 2)
                    .Select(n => new TypeWithValueCollection("Nested " + n, n, "2015-06-26 08:1" + n))
                    .ToList();
            }
        }

        class ComplexType
        {
            public List<NestedTypeWIthChildren> Children { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }

            public ComplexType(string stringValue, int intValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                Children = Enumerable.Range(0, 2)
                    .Select(n => new NestedTypeWIthChildren("Child " + n))
                    .ToList();
            }
        }

        class TypeWithNoProperties
        {
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _output = new Output();
        }

        [Test]
        public void SimpleObjectIsReported()
        {
            //Arrange
            var reporter = new ObjectReporter<SimpleType>();
            var st = new SimpleType("X", 10, "2015-06-05");

            //Act
            reporter.Report(st, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void DefaultReportTypeCanBeSpecifed()
        {
            //Arrange
            var st = new SimpleType("X", 10, "2015-06-05");

            //Act
            var reporter = new ObjectReporter<SimpleType>(ReportType.PropertyList);

            //Assert
            reporter.Report(st, _output);
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ChildObjectsAreReportedAsTablesInPropertyList()
        {
            //Arrange
            var item = new TypeWithChild("X", 10, "2015-06-05");
            var reporter = new ObjectReporter<TypeWithChild>(ReportType.PropertyList);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ChildObjectsAreReportedAsNestedReportsInATable()
        {
            //Arrange
            var item = new TypeWithChild("X", 10, "2015-06-05");
            var reporter = new ObjectReporter<TypeWithChild>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ChildCollectionsAreReportedAsNestedReportsInATable()
        {
            //Arrange
            var item = new TypeWithChildCollection("X", 10, "2015-06-05");
            var reporter = new ObjectReporter<TypeWithChildCollection>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ChildObservableCollectionsAreReportedAsNestedReportsInATable()
        {
            //Arrange
            var item = new TypeWithChildOCollection("X", 10, "2015-06-05");
            var reporter = new ObjectReporter<TypeWithChildOCollection>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void DerivedCollectionsAreReportedAsNestedReportsInATable()
        {
            //Arrange
            var item = new TypeWithChildDerivedCollection("X", 10, "2015-06-05");
            var reporter = new ObjectReporter<TypeWithChildDerivedCollection>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ValueCollectionsAreReportedAsNestedReportsInATable()
        {
            //Arrange
            var item = new TypeWithValueCollection("X", 10, "2015-06-05");
            var reporter = new ObjectReporter<TypeWithValueCollection>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void MultipleLevelsOfNestingAreReported()
        {
            //Arrange
            var item = new ComplexType("X", 10);
            var reporter = new ObjectReporter<ComplexType>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ObjectWithNoPropertiesIsReported()
        {
            //Arrange
            var item = new TypeWithNoProperties();
            var reporter = new ObjectReporter<TypeWithNoProperties>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void ObjectReporterCanReportACollection()
        {
            //Arrange
            var item = new List<string> {"Alpha", "Beta", "Charlie", "Delta", "Echo", "Foxtrot" };
            var reporter = new ObjectReporter<List<string>>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Console.WriteLine(_output.Report);
            Approvals.Verify(_output.Report);
        }

        [Test]
        public void NullObjectPropetiesAreReportedCorrectly()
        {
            //Arrange
            var item = new TypeWithChild("A", 1, "2015-07-27");
            item.Child = null;
            var reporter = new ObjectReporter<TypeWithChild>(ReportType.Table);

            //Act
            reporter.Report(item, _output);

            //Assert
            Console.WriteLine(_output.Report);
            Approvals.Verify(_output.Report);
        }
    }
}
