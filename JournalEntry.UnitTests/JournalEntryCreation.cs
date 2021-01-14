using Moq;
using System;
using Xunit;

namespace JournalEntry.UnitTests
{
    public class JournalEntryCreation
    {
        [Fact]
        public void CreateEntryInsertTODOsFromSpecifiedFile()
        {
            JournalEntry je = new JournalEntry("CreateEntryInsertTODOsFromSpecifiedFile");

            // Oops, there is nothing to Mock
            // Mock<IJournal> iJournalMoq = new Mock<IJournal>();
            
            var actualResult = je.CreateDailyEntryFile();

            // How to assert? Will I need to read file? Think so...
            Assert.Equal(EntryCreationStatus.Success, actualResult);
        }
    }
}
