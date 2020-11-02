using System.Collections.Generic;
using System.Linq;

namespace TestConsole.OutputFormatting.Internal
{
    internal static class WordSplitter
    {
        private class SplitImpl
        {
            private readonly int _tabLength;
            public readonly List<SplitWord> Words = new List<SplitWord>();
            private SplitWord _current;

            public SplitImpl(string data, int tabLength)
            {
                _tabLength = tabLength;
                var items = ControlSplitter.Split(data);
                _current = null;
                InitFromControlItems(items);
            }

            private void InitFromControlItems(IEnumerable<TextControlItem> items)
            {
                foreach (var colourControlItem in items)
                {
                    ProcessControlItem(colourControlItem);
                }
                StoreCurrentIfPresent();
            }

            private void ProcessControlItem(TextControlItem textControlItem)
            {
                if (!string.IsNullOrEmpty(textControlItem.Text))
                {
                    StoreCurrentIfPresent();

                    var splitWords = SplitWords(textControlItem.Text, _tabLength);

                    if (splitWords.Length > 1)
                        Words.AddRange(splitWords.Take(splitWords.Length - 1));

                    _current = splitWords[splitWords.Length - 1];
                }
                else
                {
                    StoreCurrentIfPresent();
                    _current = new SplitWord(0, 0, string.Empty);                    
                    _current.AddInstructions(textControlItem.Instructions);
                }
            }

            private void StoreCurrentIfPresent()
            {
                if (_current != null)
                {
                    Words.Add(_current);
                    _current = null;
                }
            }
        }

        private static readonly string WordTermChars = ",.";
        private static readonly string SpaceChars = " \t\r\n";
        private static readonly char[] SplitChars = (SpaceChars + WordTermChars).ToCharArray();

        public static IReadOnlyList<SplitWord> Split(string data, int tabLength)
        {
            return new SplitImpl(data, tabLength).Words;
        }

        private static SplitWord[] SplitWords(string data, int tabLength)
        {
            var words = new List<SplitWord>();
            var dataPos = 0;
            while (dataPos < data.Length)
            {
                var nextEnd = data.IndexOfAny(SplitChars, dataPos);
                if (nextEnd < 0)
                {
                    var length = data.Length - dataPos;
                    words.Add(new SplitWord(length, 0, data.Substring(dataPos, length)));
                    break;
                }

                if (nextEnd < data.Length && WordTermChars.Contains(data[nextEnd]))
                    ++nextEnd;

                var wordLen = nextEnd - dataPos;
                var spaces = 0;
                while (nextEnd < data.Length && SpaceChars.Contains(data[nextEnd]))
                {
                    var nextChar = data[nextEnd];
                    if (nextChar == '\r' || nextChar == '\n')
                    {
                        ++nextEnd;
                        if (nextChar == '\r' && nextEnd < data.Length && data[nextEnd] == '\n')
                        {
                            ++nextEnd;
                        }
                        break;
                    }
                    spaces += nextChar == ' ' ? 1 : tabLength;
                    ++nextEnd;
                }

                var wordValue = data.Substring(dataPos, wordLen);
                words.Add(new SplitWord(GetVisibleLength(wordValue), spaces, wordValue));
                dataPos = nextEnd;
            }
            return words.ToArray();
        }

        private static int GetVisibleLength(string wordValue)
        {
            var components = ControlSplitter.Split(wordValue);
            return components.Sum(arg => arg.Text != null ? arg.Text.Length : 0);
        }
    }
}