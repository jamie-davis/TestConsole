using System;
using System.Collections.Generic;
using System.Text;

namespace TestConsoleLib.Exceptions
{
    public class IncorrectNumberOfKeyFieldsException : Exception
    {
        public string TableName { get; }
        public List<string> ExpectedKeyFields { get; }
        public List<object> ValuesProvided { get; private set; }

        public IncorrectNumberOfKeyFieldsException(string tableName, List<string> expectedKeyFields, List<object> valuesProvided)
        : base("Incorrect number of field provided for table key.")
        {
            TableName = tableName;
            ExpectedKeyFields = expectedKeyFields;
            ValuesProvided = valuesProvided;
        }
    }
}
