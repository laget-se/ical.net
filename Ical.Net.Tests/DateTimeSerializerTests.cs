using Ical.Net.DataTypes;
using Ical.Net.Serialization.DataTypes;
using System;
using System.ComponentModel;
using Xunit;

namespace Ical.Net.Tests
{
    public class DateTimeSerializerTests
    {
        [Fact, Category("Deserialization")]
        public void TZIDPropertyShouldBeAppliedForLocalTimezones()
        {
            // see http://www.ietf.org/rfc/rfc2445.txt p.36
            var result = new DateTimeSerializer()
                .SerializeToString(
                new CalDateTime(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), "US-Eastern"));

            // TZID is applied elsewhere - just make sure this doesn't have 'Z' appended.
            Assert.Equal("19970714T133000", result);
        }
    }
}