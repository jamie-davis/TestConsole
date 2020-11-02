namespace TestConsole.OutputFormatting.Internal.RecordedCommands
{
    internal class WriteCommand : SimpleTextCommandBase, IRecordedCommand
    {
        public WriteCommand(string data, SplitCache cache) : base(data, cache)
        {
        }

        public void Replay(ReplayBuffer buffer)
        {
            buffer.Write(SplitText);
        }
    }
}
