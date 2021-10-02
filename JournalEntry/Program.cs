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

            // TODO: Add this back after getting path to subfolder worked out - or move to JournalEntry class?
            // RemoveEmptyEntries(journalEntry.FileNamePrefix);

        }

        // TODO: Should this be part of JournalEntry class, or its own removal class, or? 
        private static void RemoveEmptyEntries(string fileNamePrefix)
        {
            // Get filename of prior day
            bool foundEntry = false;
            int entriesChecked = 0; // one week?

            while (!foundEntry && entriesChecked <= 7) // todo create constant
            {
                var daysBack = -(entriesChecked + 1);

                // TODO: Extract formatting?
                var yesterday = DateTime.Now.AddDays(daysBack).ToString("MMMdd_yyyy"); 

                string journalDirectory = $"C:\\Users\\Stephen\\Documents\\Writing\\Journal";

                string filename = $"{fileNamePrefix}_{yesterday}.md";

                string fullPath = $"{journalDirectory}\\{filename}";

                // if file contains "***EOFEOF_I did not write a journal entry today_EOFEOF***" delete it
                if (File.Exists(fullPath))
                {
                    var fileInfo = new System.IO.FileInfo(fullPath);

                    long length = fileInfo.Length;

                    bool hasEmptyEntryToken = File.ReadLines(fullPath).Contains(JournalEntry.emptyEntryToken);

                    // For now, to be on the safe side, the file must be both below minimum AND have the token
                    // if ((length < JournalEntry.minimumFileSize) && (hasEmptyEntryToken))
                    // Forget about filesize for now
                    if (hasEmptyEntryToken)
                    {
                        File.Delete(fullPath);
                        Console.WriteLine($"Deleted empty entry {fullPath}."); // TODO: write to log
                    }
                    else
                    {
                        foundEntry = true; // only delete multiple entries if created on successive days
                    }
                }
                entriesChecked++;
            }
        }
    }
}
