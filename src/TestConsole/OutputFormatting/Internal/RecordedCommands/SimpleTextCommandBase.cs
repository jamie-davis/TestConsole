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
        private List<TextControlItem> _splitText;
        private List<SplitWord> _splitWords;

        public SimpleTextCommandBase(string data)
        {
            _data = data;
            _splitText = ControlSplitter.Split(data);
        }

        protected List<TextControlItem> SplitText { get { return _splitText; } }

        private void SplitTextData(int tabLength)
        {
            if (tabLength >= 0 && tabLength == _tabLength && _splitText != null)
                return;

            _tabLength = tabLength;
            _splitWords = WordSplitter.SplitToList(_splitText, tabLength);
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