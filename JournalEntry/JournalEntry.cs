using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace JournalEntry
{
    public enum EntryCreationStatus { Success, EntryExists, Failure };

    // public Tuple Location {double latitude, double longitude };

    public class JournalEntry : IJournal, ISunriseSunset
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
                // TODO: Get location another way
                string lat = "47.608013"; 
                string lon = "-122.335167";

                string sunrise = GetTodaySunrise(lat, lon);

                templateString = $"Sunrise {sunrise}\n";               

                // TODO: Figure out how to handle this, get sunrise/sunset data with one call, then second calls
                // templateString += GetTodaySunset(lat, lon);

                // This gets the TODOs at the end
                templateString += File.ReadAllText(fullPath);
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

            return sunriseLocal.ToString();
        }

        public string GetTodaySunset(string latitude, string longitude)
        {
            string sunset = "5:30pm";

            return sunset;
        }
    }
}
