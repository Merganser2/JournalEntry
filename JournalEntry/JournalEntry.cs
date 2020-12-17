using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JournalEntry
{
    public enum EntryCreationStatus { Success, EntryExists, Failure };

    public class JournalEntry : IJournal
    {
        public string FileName { get; set; }
        public string FileHeader { get; set; }

        public JournalEntry(string fileName = null, string header = null)
        {
            FileName = fileName;
            FileHeader = FileHeader;
        }

        public EntryCreationStatus CreateDailyEntryFile()
        {
            // Get the path for the log
            string journalDirectory = $"C:\\Users\\Stephen\\Documents\\Writing\\Journal";

            // TODO: Get the write Date format
            string today = DateTime.Now.ToString("MMMdd_yyyy");

            string filename = $"StephenLog_{today}.md";

            string fullPath = $"{journalDirectory}\\{filename}";

            string headerText = GetHeaderText();

            try
            {
                if (!File.Exists(fullPath))
                {
                    using (var fileStream = System.IO.File.Create(fullPath))
                    {
                        byte[] headerTextBytes = new UTF8Encoding(true).GetBytes(headerText);

                        fileStream.Write(headerTextBytes);
                    }
                }
                else
                {
                    LogJournalCreation();
                }
            }
            catch (Exception ex)
            {
                LogJournalCreation(true);
                return EntryCreationStatus.Failure;
            }
            return EntryCreationStatus.Success;
        }

        public string GetHeaderText()
        {
            string today = DateTime.Today.ToString("dddd, MMMM d, yyyy");

            string headerText = $"StephenLog {today}\n" +
                                "======================================\n\n";

                return headerText;
        }

        bool IJournal.PublishToBlog()
        {
            throw new NotImplementedException();
        }

        // TODO: Reconsider how to do this
        public bool LogJournalCreation(bool failure = false)
        {
            // Get the path for the log
            string journalDirectory = $"C:\\Users\\Stephen\\Documents\\Writing\\Journal";

            // TODO: Get the write Date format
            string today = DateTime.Now.ToString("MMMdd_yyyy");

            string statusMsg = "FailedToCreateEntry";

            string filename = $"{statusMsg}_{today}.md";

            string fullPath = $"{journalDirectory}\\{filename}";

            string headerText = failure ? "Unknown error" : "Journal entry already present"; // GetHeaderText();

            try
            {
                if (!File.Exists(fullPath))
                {
                    using (var fileStream = System.IO.File.Create(fullPath))
                    {
                        byte[] headerTextBytes = new UTF8Encoding(true).GetBytes(headerText);

                        fileStream.Write(headerTextBytes);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

    }
}
