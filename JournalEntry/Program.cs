using System;
using System.IO;
using System.Linq;

namespace JournalEntry
{
    class Program
    {
        static void Main(string[] args)
        {
            JournalEntry journalEntry = new JournalEntry();

            var outcome = journalEntry.CreateDailyEntryFile();

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
