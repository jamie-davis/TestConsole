using System.Linq;
using NUnit.Framework;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.Internal;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace TestConsole.Tests
{
    [TestFixture]
    public class TestOutput
    {
        private Tree[] _data;

        class Tree
        {
            private static string[] Descriptions = 
            {
                "{0} is a species of tree growing to a height of {1} metres.",
                "The beautiful {0} tree can grow to a height of {1} metres.",
                "At up to {1} metres tall, the {0} is a fine example of a british tree.",
                "Distinctive and evocative of a British landscape, the {0} tree is truly a {1} metre asset worth preserving"
            };

            private static int nextDesc = 0;

            public string Name { get; set; }
            public string Description { get; set; }
            public int HeightInMetres { get; set; }

            public Tree(string name, int height)
            {
                if (nextDesc == Descriptions.Length) 
                    nextDesc = 0;

                Description = string.Format(Descriptions[nextDesc++], name, height);
                Name = name;
                HeightInMetres = height;
            }
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            //Don't rely on this data for anything other than the formatting in this test
            _data = new[]
            {
                new Tree("Alder",    30),
                new Tree("Apple",    12),
                new Tree("Ash",      35),
                new Tree("Birch",    25),
                new Tree("Beech",    50),
                new Tree("Cherry",   32),
                new Tree("Elm",      40),
                new Tree("Hawthorn", 14),
                new Tree("Hazel",    8),
                new Tree("Holly",    25),
                new Tree("Juniper",  10),
                new Tree("Maple",    45),
                new Tree("Oak",      12),
                new Tree("Poplar",   20),
                new Tree("Rowan",    15),
                new Tree("Willow",   14),
                new Tree("Yew",      20),
            };
        }

        [Test]
        public void OutputIsCaptured()
        {
            //Arrange
            var output = new Output();

            //Act (runs all methods on Output)
            output.WrapLine("The number of trees is {0}", _data.Length);
            output.WriteLine();
            output.FormatTable(_data);

            var report = _data
                .OrderByDescending(t => t.HeightInMetres)
                .AsReport(rep => rep
                .AddColumn(t => t.Name, cc => { })
                .AddColumn(t => t.HeightInMetres, cc => cc.Heading("Maximum Height (m)"))
                );
            output.WriteLine();
            output.FormatTable(report);

            output.WriteLine();
            var trees = string.Join(", ", _data.Select(t => t.Name));
            output.Write("Write: {0}.", trees);
            output.WriteLine("<--");
            output.Wrap("Wrap: {0}.", trees);
            output.WriteLine("<--");
            output.WriteLine("WriteLine: {0}.", trees);
            output.WrapLine("WrapLine: {0}.", trees);

            var recording = new RecordingConsoleAdapter();
            recording.Wrap("Recorded Wrap: {0}", trees);
            output.Write(recording);
            output.WriteLine("<--");
            output.WriteLine(recording);
            
            //Assert
            Approvals.Verify(output.Report);
        }

        [Test]
        public void BufferCanBeSharedByMultipleOutputs()
        {
            //Arrange
            var buffer = new OutputBuffer {BufferWidth = 50};

            var output = new Output(buffer);
            var output2 = new Output(buffer);

            //Act (runs all methods on Output)
            output.WrapLine("First output", _data.Length);
            output2.WrapLine("Second output", _data.Length);
            output2.WriteLine();
            output.FormatTable(_data.Take(5));
            output.WriteLine();
            output2.FormatTable(_data.Skip(5).Take(5));
            
            //Assert
            Approvals.Verify(buffer.GetBuffer());
        }

    }
}
