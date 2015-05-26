namespace TestConsole.OutputFormatting.Internal.RecordedCommands
{
    internal class WriteCommand : SimpleTextCommandBase, IRecordedCommand
    {
        public WriteCommand(string data) : base(data)
        {
        }

        public void Replay(ReplayBuffer buffer)
        {
            buffer.Write(SplitText);
        }
    }
}
