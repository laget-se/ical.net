using Ical.Net.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ical.Net.Tests
{
    public class CollectionHelpersTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;
        private static readonly DateTime _later = _now.AddHours(1);
        private static readonly string _uid = Guid.NewGuid().ToString();

        private static List<RecurrencePattern> GetSimpleRecurrenceList()
            => new List<RecurrencePattern> { new RecurrencePattern(FrequencyType.Daily, 1) { Count = 5 } };
        private static List<PeriodList> GetExceptionDates()
            => new List<PeriodList> { new PeriodList { new Period(new CalDateTime(_now.AddDays(1).Date)) } };

        [Fact]
        public void ExDateTests()
        {
            Assert.Equal(GetExceptionDates(), GetExceptionDates());
            Assert.NotEqual(GetExceptionDates(), null);
            Assert.NotEqual(null, GetExceptionDates());

            var changedPeriod = GetExceptionDates();
            changedPeriod.First().First().StartTime = new CalDateTime(_now.AddHours(-1));

            Assert.NotEqual(GetExceptionDates(), changedPeriod);
        }
    }
}
