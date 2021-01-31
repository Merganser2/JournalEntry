using System;
using System.Collections.Generic;
using System.Text;

namespace JournalEntry
{
    public interface ISunriseSunset
    {
        public string GetTodaySunrise(string latitude, string longitude);

        public string GetTodaySunset(string latitude, string longitude);

    }
}
