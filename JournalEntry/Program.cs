using System;

namespace JournalEntry
{
    class Program
    {
        static void Main(string[] args)
        {
            JournalEntry journalEntry = new JournalEntry();

            var outcome = journalEntry.CreateDailyEntryFile();

            // TODO: Find better way to do this? 
            // ALSO: journalEntry.CreationStatus(); 
            string status;
            if (outcome == EntryCreationStatus.Success)
            {
                status = "Journal Entry created";
            }
            else if (outcome == EntryCreationStatus.EntryExists)
            {
                status = "Today's journal entry file already exists";
            }
            else // (outcome == EntryCreationStatus.Failure)
            {
                status = "Error: Failed to create journal entry";
            }

            Console.WriteLine(status);
        }
    }
}
