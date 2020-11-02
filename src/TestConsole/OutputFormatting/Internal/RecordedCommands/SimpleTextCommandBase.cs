using System.Collections.Generic;
using System.Linq;

namespace TestConsole.OutputFormatting.Internal.RecordedCommands
{
    internal class SimpleTextCommandBase
    {
        protected string _data;

        private int _tabLength = -1;
        private int _firstWordLength;
        private int _longestWordLength; 
        private IReadOnlyList<SplitWord> _splitWords;
        private readonly SplitCache _cache;

        public SimpleTextCommandBase(string data, SplitCache cache)
        {
            _data = data;
            _cache = cache;
        }

        protected List<TextControlItem> SplitText => ControlSplitter.Split(_data);

        private void SplitTextData(int tabLength)
        {
            if (tabLength >= 0 && tabLength == _tabLength)
                return;

            _tabLength = tabLength;
            _splitWords = _cache.Split(_data, tabLength);
            var firstWord = _splitWords.FirstOrDefault();

            _firstWordLength = firstWord == null ? 0 : firstWord.Length;
            _longestWordLength = _splitWords.Any() ? _splitWords.Max(w => w.Length) : 0;
        }

        public int GetFirstWordLength(int tabLength)
        {
            if (tabLength == _tabLength && tabLength >= 0)
                return _firstWordLength;

            SplitTextData(tabLength);
            return _firstWordLength;
        }

        public int GetLongestWordLength(int tabLength)
        {
            if (tabLength == _tabLength && tabLength >= 0)
                return _longestWordLength;

            SplitTextData(tabLength);
            return _longestWordLength;
        }
    }
}