using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestConsole.OutputFormatting.Internal
{
    public static class PropertyNameConverter
    {
        public static string ToHeading(PropertyInfo prop)
        {
            var name = prop.Name;
            return ToHeading(name);
        }

        public static string ToHeading(string name)
        {
            if (name.All(char.IsUpper))
                return name;

            var words = new List<string>();
            string word = null;
            foreach (var character in name)
            {
                if (char.IsUpper(character))
                {
                    if (word != null)
                    {
                        words.Add(word);
                        word = null;
                    }
                }

                if (word == null)
                    word = character.ToString();
                else
                    word += character;
            }

            if (word != null)
                words.Add(word);

            return string.Join(" ", words);
        }

        public static string ToPrompt(PropertyInfo prop)
        {
            if (prop == null) return null;

            return ToHeading(prop);
        }
    }
}