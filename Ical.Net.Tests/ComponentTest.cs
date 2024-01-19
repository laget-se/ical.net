using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using System;
using System.ComponentModel;
using Xunit;

namespace Ical.Net.Tests
{
    public class ComponentTest
    {
        [Fact, Category("Components")]
        public void UniqueComponent1()
        {
            var iCal = new Calendar();
            var evt = iCal.Create<CalendarEvent>();

            Assert.NotNull(evt.Uid);
            Assert.Null(evt.Created); // We don't want this to be set automatically
            Assert.NotNull(evt.DtStamp);
        }

        [Fact, Category("Components")]
        public void ChangeCalDateTimeValue()
        {
            var e = new CalendarEvent
            {
                Start = new CalDateTime(2017, 11, 22, 11, 00, 01),
                End = new CalDateTime(2017, 11, 22, 11, 30, 01),
            };

            var firstStartAsUtc = e.Start.AsUtc;
            var firstEndAsUtc = e.End.AsUtc;

            e.Start.Value = new DateTime(2017, 11, 22, 11, 30, 01);
            e.End.Value = new DateTime(2017, 11, 22, 12, 00, 01);

            var secondStartAsUtc = e.Start.AsUtc;
            var secondEndAsUtc = e.End.AsUtc;

            Assert.NotEqual(firstStartAsUtc, secondStartAsUtc);
            Assert.NotEqual(firstEndAsUtc, secondEndAsUtc);
        }
    }
}
