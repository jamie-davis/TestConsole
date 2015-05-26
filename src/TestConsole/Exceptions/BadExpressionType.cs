using System;
using System.Linq.Expressions;

namespace TestConsole.Exceptions
{
    internal class BadExpressionType : Exception
    {
        public BadExpressionType(Expression expression)
            : base(string.Format("The expression \"{0}\" is not acceptable in a report.", expression))
        {
        }
    }
}