using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    [TestFixture]
    public class TestSplitCache
    {
        [Test]
        public void TextIsSplit()
        {
            //Arrange
            var cache = new SplitCache();
            const string testPhrase = "one two three";
           
            //Act
            var words = cache.Split(testPhrase, 4);

            //Assert
            words.Count.Should().Be(3);
        }

        [Test]
        public void SplitIsReturnedFromCache()
        {
            //Arrange
            var cache = new SplitCache();
            var testPhrases = new []
            {
                "one two three",
                "four five six",
                "seven eight nine",
            } ;
           
            var results = testPhrases.Select(tp => new { Phrase = tp, Split = cache.Split(tp, 4)}).ToList();

            //Act/Assert
            Enumerable.Range(0,10)
                .Select(n => results.All(r => ReferenceEquals(cache.Split(r.Phrase, 4), r.Split)))
                .Should().AllBeEquivalentTo(true);
        }
    }
}