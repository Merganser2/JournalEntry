using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JournalEntry
{
    public enum EntryCreationStatus { Success, EntryExists, Failure };

    public class JournalEntry : IJournal
    {
        public string FileNamePrefix { get; set; }
        public string FileHeader { get; set; }

        public string JournalDirectory { get; private set; }

        public string TemplateFile { get; private set; }

        // TODO: Move to config file? and fix these *Sfh dependencies
        private const string designatedJournalDirectory = "C:\\Users\\Stephen\\Documents\\Writing\\Journal";
        private const string templateFileSfh = "Journal_TODOs.md";
        private const string fileNamePrefixSfh = "StephenLog_";

        // TODO: fix 
        public JournalEntry(string fileNamePrefix = fileNamePrefixSfh,
                            string journalDirectory = designatedJournalDirectory, 
                            string templateFile = templateFileSfh)
        {
            FileNamePrefix = fileNamePrefix;
            // Designate the path for the log
            JournalDirectory = journalDirectory;
            TemplateFile = templateFile;
        }

        public EntryCreationStatus CreateDailyEntryFile()
        {
            string today = DateTime.Now.ToString("MMMdd_yyyy");
            string fileName = $"{FileNamePrefix}_{today}.md";

            string fullPath = $"{JournalDirectory}\\{fileName}";

            string headerText = GetHeaderText();

            string templateText = GetTemplate();

            try
            {
                if (!File.Exists(fullPath))
                {
                    using (var fileStream = System.IO.File.Create(fullPath))
                    {
                        byte[] headerTextBytes = new UTF8Encoding(true).GetBytes(headerText);
                        byte[] templateTextBytes = new UTF8Encoding(true).GetBytes(templateText);

                        fileStream.Write(headerTextBytes);

                        fileStream.Write(templateTextBytes);
                    }
                }
                else
                {
                    LogJournalCreation();
                    return EntryCreationStatus.EntryExists;
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

        public string GetTemplate()
        {
            string templateString = null;

            string fullPath = $"{JournalDirectory}\\{TemplateFile}";

            try
            {
                templateString = File.ReadAllText(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return templateString;
        }
    }
}
