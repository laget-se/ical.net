using System;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace Ical.Net.Tests
{
    public class JournalTest
    {
        [Fact, Category("Journal")]
        public void Journal1()
        {
            var iCal = Calendar.Load(IcsFiles.Journal1);
            ProgramTest.TestCal(iCal);
            Assert.Equal(1, iCal.Journals.Count);
            var j = iCal.Journals[0];

            Assert.NotNull(j, "Journal entry was null");
            Assert.Equal(JournalStatus.Draft, j.Status, "Journal entry should have been in DRAFT status, but it was in " + j.Status + " status.");
            Assert.Equal("PUBLIC", j.Class, "Journal class should have been PUBLIC, but was " + j.Class + ".");
            Assert.Null(j.Start);
        }

        [Fact, Category("Journal")]
        public void Journal2()
        {
            var iCal = Calendar.Load(IcsFiles.Journal2);
            ProgramTest.TestCal(iCal);
            Assert.Equal(1, iCal.Journals.Count);
            var j = iCal.Journals.First();

            Assert.NotNull(j, "Journal entry was null");
            Assert.Equal(JournalStatus.Final, j.Status, "Journal entry should have been in FINAL status, but it was in " + j.Status + " status.");
            Assert.Equal("PRIVATE", j.Class, "Journal class should have been PRIVATE, but was " + j.Class + ".");
            Assert.Equal("JohnSmith", j.Organizer.CommonName, "Organizer common name should have been JohnSmith, but was " + j.Organizer.CommonName);
            Assert.True(
                string.Equals(
                    j.Organizer.SentBy.OriginalString,
                    "mailto:jane_doe@host.com",
                    StringComparison.OrdinalIgnoreCase),
                "Organizer should have had been SENT-BY 'mailto:jane_doe@host.com'; it was sent by '" + j.Organizer.SentBy + "'");
            Assert.True(
                string.Equals(
                    j.Organizer.DirectoryEntry.OriginalString,
                    "ldap://host.com:6666/o=3DDC%20Associates,c=3DUS??(cn=3DJohn%20Smith)",
                    StringComparison.OrdinalIgnoreCase),
                "Organizer's directory entry should have been 'ldap://host.com:6666/o=3DDC%20Associates,c=3DUS??(cn=3DJohn%20Smith)', but it was '" + j.Organizer.DirectoryEntry + "'");
            Assert.Equal(
                "MAILTO:jsmith@host.com",
                j.Organizer.Value.OriginalString);
            Assert.Equal(
                "jsmith",
                j.Organizer.Value.UserInfo);
            Assert.Equal(
                "host.com",
                j.Organizer.Value.Host);
            Assert.Null(j.Start);
        }
    }
}
