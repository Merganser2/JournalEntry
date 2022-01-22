using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace JournalEntry
{
    public enum EntryCreationStatus { Success, EntryExists, Failure };

    // public Tuple Location {double latitude, double longitude };


    public class JournalEntry : IJournal, ISunriseSunset
    {
        public static readonly string emptyEntryToken = "***EOFEOF_No Journal Entry Today_EOFEOF***";
        public static readonly int minimumFileSize = 687;

        public string FileNamePrefix { get; set; }
        public string FileHeader { get; set; }

        public string JournalDirectory { get; private set; }

        public string TemplateFilePath { get; private set; }

        // TODO: Move to config file? and fix these *Sfh dependencies
        private const string baseJournalDirectory = "C:\\Users\\Stephen\\Documents\\Writing\\Journal";
        private const string templateFileSfh = "Journal_TODOs.md";
        private const string fileNamePrefixSfh = "StephenLog_";

        // TODO: fix 
        public JournalEntry(string fileNamePrefix = fileNamePrefixSfh,
                            string journalDirectory = baseJournalDirectory, 
                            string templateFile = templateFileSfh)
        {
            FileNamePrefix = fileNamePrefix;
            // Designate the path for the log
            JournalDirectory = GetJournalDirectory(baseJournalDirectory); // journalDirectory;
            TemplateFilePath = $"{baseJournalDirectory}\\{templateFile}";
        }

        public EntryCreationStatus CreateDailyEntryFile()
        {
            string today = DateTime.Now.ToString("MMMdd_yyyy");
            string fileName = $"{FileNamePrefix}_{today}.md";

            string fullPath = $"{JournalDirectory}\\{fileName}";

            string headerText = GetHeaderText();

            string templateText = GetTemplate();

            EntryCreationStatus creationStatus = EntryCreationStatus.Success;

            try
            {
                if (!Directory.Exists(JournalDirectory))
                    Directory.CreateDirectory(JournalDirectory);
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
                    EntryNotCreated();
                    creationStatus = EntryCreationStatus.EntryExists;
                }
            }
            catch 
            {
                EntryNotCreated(true);
                return EntryCreationStatus.Failure;
            }

            // TODO: Separate out and call from Main somehow
            // Removes prior entries but only if successive days 
            RemoveEmptyEntries(FileNamePrefix);

            return creationStatus;
        }

        public string GetHeaderText()
        {
            string today = DateTime.Today.ToString("dddd, MMMM d, yyyy");

            string headerText = $"StephenLog {today}\n" +
                                "======================================\n\n";
            headerText += $"{JournalEntry.emptyEntryToken}\n";  

            return headerText;
        }

        bool IJournal.PublishToBlog()
        {
            throw new NotImplementedException();
        }

        public bool EntryNotCreated(bool failure = false)
        {
//            string journalDirectory = $"C:\\Users\\Stephen\\Documents\\Writing\\Journal";

            string today = DateTime.Now.ToString("MMMdd_yyyy");

            string statusMsg = "FailedToCreateEntry";

            string filename = $"{statusMsg}_{today}.md";

            string fullPath = $"{JournalDirectory}\\{filename}";

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

            try
            {
                // TODO: Get location another way
                string lat = "47.608013"; 
                string lon = "-122.335167";

                string sunrise = GetTodaySunrise(lat, lon);

                templateString = $"Sunrise {sunrise}\n";               

                // TODO: Figure out how to handle this, get sunrise/sunset data with one call, then second calls
                // templateString += GetTodaySunset(lat, lon);

                // This gets the TODOs at the end
                templateString += File.ReadAllText(TemplateFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return templateString;
        }

        public string GetTodaySunrise(string latitude, string longitude)
        {
            // string sunriseURL = "https://api.sunrise-sunset.org/json";
            string sunRiseSetResponse;

            // https://api.sunrise-sunset.org/json?lat=47.608013&lng=-122.335167
            string sunriseSunsetBaseUrl = "https://api.sunrise-sunset.org/json?";
            string sunriseSunsetURL = $"{sunriseSunsetBaseUrl}lat={latitude}&lng={longitude}";

            using (var downloader = new System.Net.WebClient())
            {
                sunRiseSetResponse = downloader.DownloadString(sunriseSunsetURL);
                //                var json = sunriseURL
            }

            // Convert to a JSON object and find the "page" element's value
            JObject sunRiseSetJsonObject = JObject.Parse(sunRiseSetResponse);
            string pathToSunRiseData = "results.sunrise";
            JToken sunriseToken = sunRiseSetJsonObject.SelectToken(pathToSunRiseData);

            string sunriseUtc = sunriseToken?.ToString(); // .Replace("a. m.", "AM").Replace("p. m.", "PM");
            
            string sunriseFormatString = "h:mm:ss tt"; // tt is for the AM/PM

            DateTime sunriseUtcDateTime = DateTime.ParseExact(sunriseUtc, sunriseFormatString, CultureInfo.InvariantCulture);

            var sunriseLocal = TimeZoneInfo.ConvertTimeFromUtc(sunriseUtcDateTime, TimeZoneInfo.Local);

            return sunriseLocal.ToShortTimeString();
        }

        public string GetTodaySunset(string latitude, string longitude)
        {
            string sunset = "5:30pm";

            return sunset;
        }

//        public enum MonthFolder { JanFeb, MarchApril, MayJune, JulyAug, SeptOct, NovDec };
        static string[] FolderNames = { "JanFeb", "MarchApril", "MayJune", "JulyAug", "SeptOct", "NovDec" };

        private string GetJournalDirectory(string baseDirectory)
        {
            var today = DateTime.Today;
            int month = today.Month;
            string year = today.ToString("yy");

            string monthFolder = getMonthDirectoryName(month);

            string dirSuffix = $"{monthFolder}_{year}";

            return baseDirectory + "\\" + dirSuffix;
        }

        // TODO: Write a unit test.  1/2 = 1, 2/2 = 1, 3/2 = 2, etc...
        private string getMonthDirectoryName(int month)
        {
            int monthGroup = ((month + 1)/ 2) - 1; 
 
            return FolderNames[monthGroup];
        }

        private void RemoveEmptyEntries(string fileNamePrefix)
        {
            // Get filename of prior day
            bool foundEntry = false;
            int entriesChecked = 0; // one week?

            while (!foundEntry && entriesChecked <= 7) // todo create constant
            {
                var daysBack = -(entriesChecked + 1);

                // TODO: Extract formatting?
                var yesterday = DateTime.Now.AddDays(daysBack).ToString("MMMdd_yyyy");

                // string journalDirectory = $"C:\\Users\\Stephen\\Documents\\Writing\\Journal";

                string filename = $"{fileNamePrefix}_{yesterday}.md";

                string fullPath = $"{JournalDirectory}\\{filename}";

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
