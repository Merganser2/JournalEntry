﻿using System;
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

            RemoveEmptyEntries(journalEntry.FileNamePrefix);

        }

        // Should this be part of JournalEntry class, or?? TODO:
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

                    // TODO: Move these elsewhere
                    string emptyEntryToken = "***EOFEOF_I did not write a journal entry today_EOFEOF***";
                    int minimumFileSize = 1159;

                    bool hasEmptyEntryToken = File.ReadLines(fullPath).Contains(emptyEntryToken);

                    if ((length < minimumFileSize) || (hasEmptyEntryToken))
                    {

                        File.Delete(fullPath);
                    }                    
                    else
                    {
                        foundEntry = true; // only delete multiple entries if created on successive days
                    }
                }
                entriesChecked++;
            }

            // Read file and check filesize? and string indicating nothing entered

        }
    }
}
