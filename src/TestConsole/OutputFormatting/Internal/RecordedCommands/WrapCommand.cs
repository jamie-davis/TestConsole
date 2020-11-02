namespace TestConsole.OutputFormatting.Internal.RecordedCommands
{
    internal class WrapCommand : SimpleTextCommandBase, IRecordedCommand
    {
        public WrapCommand(string data, SplitCache cache) : base(data, cache)
        {
        }

        public void Replay(ReplayBuffer buffer)
        {
            buffer.Wrap(_data);
        }
    }
}