using System.Reflection;

namespace TestConsole.OutputFormatting.Internal
{
    /// <summary>
    /// Defines a property source
    /// </summary>
    internal interface IPropertySource
    {
        PropertyInfo Property { get; }
        object Value { get; }
    }
}