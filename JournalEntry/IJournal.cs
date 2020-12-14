namespace JournalEntry
{
    public interface IJournal
    {
        public string GetHeaderText();

        public EntryCreationStatus CreateDailyEntryFile();

        public bool PublishToBlog(); // Move to other interface?
    }
}