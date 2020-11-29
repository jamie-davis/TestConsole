using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.OutputFormatting.Internal
{
    /*
     * Notes about splitting text for wrapping:
     * 
     * Initially, the problem was splitting plain text. It was simple, because white space was simply a separator for words,
     * and the blanks could therefore be assigned to the word that precedes them and they could later be dropped if the word
     * is last on the line.
     * 
     * However, introducing colour instructions adds a great deal of complexity.
     * 
     * For example:
     * 
     * "The dog   ate the cat."
     * 
     * Becomes:
     * 
     * The:1
     * dog:3
     * ate:1
     * the:1
     * cat.:0
     * 
     * (In this notation, the text is followed by a colon, and then the number of spaces that separate the word from the next.)
     * 
     * The rule for the wrapping logic will be that whitespace that is to be skipped has the spaces removed, but the rest
     * of the component remains intact. Whether the formatting information is part of the end of the previous line, or the
     * beginning of the next line is a problem for the wrapper. Arguably, however, the beginning of the text that followed the
     * whitespace is probably the most appropriate position. This will only change the output if spaces were included at the
     * beginning or end of a block of coloured text.
    */

    [TestFixture]
    public class TestWordSplitter
    {
        [Test]
        public void WordsAreExractedWithTrailingSpaceCounts()
        {
            const string testPhrase = "one two  three   none.";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]"));
        }

        [Test]
        public void TabsAreConvertedIntoSpaces()
        {
            const string testPhrase = "one\ttwo\t three \t eight.\t\t";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,4,""one""][3,5,""two""][5,6,""three""][6,8,""eight.""]"));
        }

        [Test]
        public void WordsAreExtracted()
        {
            const string testPhrase = "one\t two  three\r\nfour\rfive\nsix";
            var words = WordSplitter.Split(testPhrase, 4);
            DescribeWords(words).Verify();
        }

        [Test]
        public void LinesAreExtractedIfWordWrappingIsOff()
        {
            const string testPhrase = "one\t two  three\r\n\tfour\rfive  \nsix";
            var words = WordSplitter.Split(testPhrase, 4, false);
            DescribeWords(words).Verify();
        }

        [Test]
        public void NewlinesCreateTerminatesLineWord()
        {
            const string testPhrase = "one\ttwo\t \nthree \t eight.\t\t\r";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected = 
                     "[3,4,T:no]"                   // one
                   + "[3,5,T:no]"                   // two
                   + "[0,0,T:yes<(NewLine)]"  // \n
                   + "[5,6,T:no]"                   // three
                   + "[6,8,T:no]"                   // eight.
                   + "[0,0,T:yes<(NewLine)]"  // \r
                   ;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void NewlinesAreStrippedFromWords()
        {
            const string testPhrase = "text\n\nmore";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            Assert.That(words.Select(w => w.WordValue).ToArray(), Is.EqualTo(new [] { "text", string.Empty, string.Empty, "more"}));
        }

        [Test]
        public void CrLfCountsAsOneLineEnd()
        {
            const string testPhrase = "one\r\ntwo\r\n\r\nthree\n\n\r\r";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected =
                  "[3,0,T:no]"                    // one
                + "[0,0,T:yes<(NewLine)]"   // \r\n
                + "[3,0,T:no]"                    // two
                + "[0,0,T:yes<(NewLine)]"   // \r\n
                + "[0,0,T:yes<(NewLine)]"   // \r\n
                + "[5,0,T:no]"                    // three
                + "[0,0,T:yes<(NewLine)]"   // \n
                + "[0,0,T:yes<(NewLine)]"   // \n
                + "[0,0,T:yes<(NewLine)]"   // \r
                + "[0,0,T:yes<(NewLine)]"   // \r
                ;

            Assert.That(result, Is.EqualTo(expected));
        }

        private static string DescribeWords(IEnumerable<SplitWord> words)
        {
            return words
                .Select(wo => string.Format(@"[{0},{1},""{2}""]", wo.Length, wo.TrailingSpaces, wo.WordValue))
                .Aggregate((t, i) => t + i);
        }

        private static string DescribeWords2(IEnumerable<SplitWord> words)
        {
            return words
                .Select(wo => string.Format("[{0},{1},T:{2}{3}]", 
                    wo.Length, wo.TrailingSpaces, 
                    wo.TerminatesLine() ? "yes" : "no",
                    FormatInstructions(wo)))
                .Aggregate((t, i) => t + i);
        }

        private static string FormatInstructions(SplitWord splitWord)
        {
            var output = string.Empty;
            if (splitWord.Instructions.Any())
                output += "<" + FormatInstructions(splitWord.Instructions);

            return output;
        }

        private static string FormatInstructions(IEnumerable<TextControlItem.ControlInstruction>instructions)
        {
            return string.Format("({0})", string.Join(",", instructions.Select(i => i.ToString())));
        }
    }
}