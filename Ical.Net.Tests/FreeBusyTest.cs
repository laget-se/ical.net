﻿using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using System.ComponentModel;
using Xunit;

namespace Ical.Net.Tests
{
    public class FreeBusyTest
    {
        /// <summary>
        /// Ensures that GetFreeBusyStatus() return the correct status.
        /// </summary>
        [Fact, Category("FreeBusy")]
        public void GetFreeBusyStatus1()
        {
            Calendar cal = new Calendar();

            CalendarEvent evt = cal.Create<CalendarEvent>();
            evt.Summary = "Test event";
            evt.Start = new CalDateTime(2010, 10, 1, 8, 0, 0);
            evt.End = new CalDateTime(2010, 10, 1, 9, 0, 0);

            var freeBusy = cal.GetFreeBusy(new CalDateTime(2010, 10, 1, 0, 0, 0), new CalDateTime(2010, 10, 7, 11, 59, 59));
            Assert.Equal(FreeBusyStatus.Free, freeBusy.GetFreeBusyStatus(new CalDateTime(2010, 10, 1, 7, 59, 59)));
            Assert.Equal(FreeBusyStatus.Busy, freeBusy.GetFreeBusyStatus(new CalDateTime(2010, 10, 1, 8, 0, 0)));
            Assert.Equal(FreeBusyStatus.Busy, freeBusy.GetFreeBusyStatus(new CalDateTime(2010, 10, 1, 8, 59, 59)));
            Assert.Equal(FreeBusyStatus.Free, freeBusy.GetFreeBusyStatus(new CalDateTime(2010, 10, 1, 9, 0, 0)));
        }
    }
}
