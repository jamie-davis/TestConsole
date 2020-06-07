using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestConsoleLib.Snapshots
{
    internal static class SnapshotDefinitionLoader
    {
        internal static DefinitionSet Load(Assembly assembly, Func<Type, bool> typeFilter = null)
        {
            var types = assembly.GetTypes()
                .Where(t => !t.IsNested && t.GetCustomAttribute<SnapshotDefinitionAttribute>() != null)
                .Where(t => typeFilter == null || typeFilter(t))
                .ToList();
            return LoadDefinitions(types);
        }

        public static DefinitionSet Load(Type containerType, Func<Type, bool> typeFilter = null)
        {
            var types = containerType.GetNestedTypes()
                .Where(t => t.GetCustomAttribute<SnapshotDefinitionAttribute>() != null)
                .Where(t => typeFilter == null || typeFilter(t))
                .ToList();
            return LoadDefinitions(types);
        }

        private static DefinitionSet LoadDefinitions(List<Type> types)
        {
            var attributed = types.Select(t => new
                    {Type = t, Attribute = t.GetCustomAttribute<SnapshotDefinitionAttribute>()})
                .Where(t => t.Attribute != null)
                .Select(t => ExtractDefinition(t.Type, t.Attribute.SnapshotTableName))
                .Where(d => d != null);
            return new DefinitionSet(attributed);
        }

        private static TableDefinition ExtractDefinition(Type type, string snapshotTableName)
        {
            if (snapshotTableName == null)
                return null;

            var definition = new TableDefinition(snapshotTableName)
            {
                DefiningType = type
            };

            var propertyAttributes = type.GetProperties()
                .Select(p => new
                {
                    Prop = p,
                    Unpredictable = p.GetCustomAttribute<UnpredictableAttribute>(),
                    Key = p.GetCustomAttribute<KeyAttribute>(),
                    References = p.GetCustomAttribute<ReferencesAttribute>(),
                })
                .ToList();

            foreach (var attributes in propertyAttributes)
            {
                if (attributes.Key != null) 
                    definition.SetPrimaryKey(attributes.Prop.Name);

                if (attributes.Unpredictable != null)
                    definition.SetUnpredictable(attributes.Prop.Name);

                if (attributes.References != null)
                    definition.SetReference(attributes.Prop.Name, attributes.References.SnapshotTableName, attributes.References.KeyPropertyName);
            }

            return definition;
        }
    }
}
