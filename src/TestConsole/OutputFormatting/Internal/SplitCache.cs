using System.Collections.Generic;

namespace TestConsole.OutputFormatting.Internal
{
    /// <summary>
    /// This class holds the results of split operations so that they can be returned when the same text needs to be split again.
    /// This was added as a performance improvement, as some long running operations need to see the split versions of column
    /// values many times.
    /// </summary>
    internal class SplitCache
    {
        private Dictionary<string, IReadOnlyList<SplitWord>> _cache = new Dictionary<string, IReadOnlyList<SplitWord>>();

        public IReadOnlyList<SplitWord> Split(string data, int tabLength)
        {
            if (!_cache.TryGetValue(data, out var split))
            {
                split = WordSplitter.Split(data, tabLength);
                _cache[data] = split;
            }

            return split;
        }
    }
}