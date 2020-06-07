namespace TestConsoleLib.Snapshots
{
    public sealed class TableDefiner
    {
        private readonly TableDefinition _tableDefinition;

        internal TableDefiner(TableDefinition tableDefinition)
        {
            _tableDefinition = tableDefinition;
        }

        public TableDefiner PrimaryKey(string fieldName)
        {
            _tableDefinition.SetPrimaryKey(fieldName);
            return this;
        }

        public TableDefiner CompareKey(string fieldName)
        {
            _tableDefinition.SetCompareKey(fieldName);
            return this;
        }

        public TableDefiner IsUnpredictable(string fieldName)
        {
            _tableDefinition.SetUnpredictable(fieldName);
            return this;
        }
    }
}   