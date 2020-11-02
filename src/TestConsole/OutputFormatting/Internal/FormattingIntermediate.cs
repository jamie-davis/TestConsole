using System;
using System.Collections.Generic;
using System.Linq;

namespace TestConsole.OutputFormatting.Internal
{
    /// <summary>
    /// A union of the types that can be format values. Most types will be strings by the time they are placed in this class, 
    /// but more specialised types may also be specified as column values.
    /// <seealso cref="IConsoleRenderer"/>
    /// </summary>
    internal class FormattingIntermediate
    {
        private readonly SplitCache _splitCache;
        private Dictionary<int, IReadOnlyList<SplitWord>> _splitWords = new Dictionary<int, IReadOnlyList<SplitWord>>();
        public string TextValue { get; private set; }
        public IConsoleRenderer RenderableValue { get; private set; }

        public int Width
        {
            get
            {
                if (TextValue != null)
                    return TextValue.Length;

                return GetLongestWordLength(0);
            }
        }

        /// <summary>
        /// Construct from a string.
        /// </summary>
        public FormattingIntermediate(string text, SplitCache splitCache)
        {
            _splitCache = splitCache;
            TextValue = text;
        }

        /// <summary>
        /// Construct from an object derived from <see cref="IConsoleRenderer"/>.
        /// </summary>
        public FormattingIntermediate(IConsoleRenderer renderableValue, SplitCache splitCache)
        {
            _splitCache = splitCache;
            RenderableValue = renderableValue;
        }

        /// <summary>
        /// Determine the length of the longest word in the value.
        /// </summary>
        /// <param name="tabLength"></param>
        /// <returns>The length of the longest word.</returns>
        public int GetLongestWordLength(int tabLength)
        {
            if (TextValue != null)
            {
                if (TextValue == string.Empty) return 0;
                return _splitCache.Split(TextValue, tabLength).Max(w => w.Length);
            }

            return RenderableValue.GetLongestWordLength(tabLength);
        }

        /// <summary>
        /// Determine the length of the first word.
        /// </summary>
        /// <returns>The length of the first word.</returns>
        public int GetFirstWordLength(int tabLength, int hangingIndent)
        {
            if (TextValue != null)
            {
                var word = _splitCache.Split(TextValue, tabLength).FirstOrDefault();
                return hangingIndent + (word == null ? 0 : word.Length);
            }

            return RenderableValue.GetFirstWordLength(tabLength);
        }

        public IEnumerable<SplitWord> ValueWords(int columnWidth, int tabLength)
        {
            IReadOnlyList<SplitWord> splitWords;
            if (_splitWords.TryGetValue(tabLength, out splitWords))
                return splitWords;

            if (RenderableValue == null)
            {
                splitWords = _splitCache.Split(TextValue, tabLength);
                _splitWords[tabLength] = splitWords;
                return splitWords;
            }

            return _splitCache.Split(ToString(columnWidth), tabLength);
        }

        /// <summary>
        /// Implicitely convert strings into intermediates. This is only here to make 
        /// construction of unit test data more succinct.
        /// </summary>
        /// <param name="text">The string the hold in the <see cref="FormattingIntermediate"/>.</param>
        /// <returns>A new <see cref="FormattingIntermediate"/>.</returns>
        public static implicit operator FormattingIntermediate(string text)
        {
            return new FormattingIntermediate(text, new SplitCache());
        }

        public override string ToString()
        {
            if (TextValue != null)
                return TextValue;

            return ToString(50);
        }

        public string ToString(int renderWidth)
        {
            if (TextValue != null)
                return TextValue;

            int wrappedLines;
            return string.Join(Environment.NewLine, RenderableValue.Render(renderWidth, out wrappedLines));
        }
    }
}