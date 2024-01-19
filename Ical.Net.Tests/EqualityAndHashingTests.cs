using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Ical.Net.Tests
{
    public class EqualityAndHashingTests
    {
        private const string _someTz = "America/Los_Angeles";
        private static readonly DateTime _nowTime = DateTime.Parse("2016-07-16T16:47:02.9310521-04:00");
        private static readonly DateTime _later = _nowTime.AddHours(1);

        [Fact, TestCaseSource(nameof(CalDateTime_TestCases))]
        public void CalDateTime_Tests(CalDateTime incomingDt, CalDateTime expectedDt)
        {
            Assert.Equal(incomingDt.Value, expectedDt.Value);
            Assert.Equal(incomingDt.GetHashCode(), expectedDt.GetHashCode());
            Assert.Equal(incomingDt.TzId, expectedDt.TzId);
            Assert.True(incomingDt.Equals(expectedDt));
        }

        public static IEnumerable<ITestCaseData> CalDateTime_TestCases()
        {
            var nowCalDt = new CalDateTime(_nowTime);
            yield return new TestCaseData(nowCalDt, new CalDateTime(_nowTime)).SetName("Now, no time zone");

            var nowCalDtWithTz = new CalDateTime(_nowTime, _someTz);
            yield return new TestCaseData(nowCalDtWithTz, new CalDateTime(_nowTime, _someTz)).SetName("Now, with time zone");
        }

        [Fact]
        public void RecurrencePatternTests()
        {
            var patternA = GetSimpleRecurrencePattern();
            var patternB = GetSimpleRecurrencePattern();

            Assert.Equal(patternA, patternB);
            Assert.Equal(patternA.GetHashCode(), patternB.GetHashCode());
        }

        [Fact, TestCaseSource(nameof(Event_TestCases))]
        public void Event_Tests(CalendarEvent incoming, CalendarEvent expected)
        {
            Assert.Equal(incoming.DtStart, expected.DtStart);
            Assert.Equal(incoming.DtEnd, expected.DtEnd);
            Assert.Equal(incoming.Location, expected.Location);
            Assert.Equal(incoming.Status, expected.Status);
            Assert.Equal(incoming.IsActive, expected.IsActive);
            Assert.Equal(incoming.Duration, expected.Duration);
            Assert.Equal(incoming.Transparency, expected.Transparency);
            Assert.Equal(incoming.GetHashCode(), expected.GetHashCode());
            Assert.True(incoming.Equals(expected));
        }

        private static RecurrencePattern GetSimpleRecurrencePattern() => new RecurrencePattern(FrequencyType.Daily, 1)
        {
            Count = 5
        };

        private static CalendarEvent GetSimpleEvent() => new CalendarEvent
        {
            DtStart = new CalDateTime(_nowTime),
            DtEnd = new CalDateTime(_later),
            Duration = TimeSpan.FromHours(1),
        };

        private static string SerializeEvent(CalendarEvent e) => new CalendarSerializer().SerializeToString(new Calendar { Events = { e } });


        public static IEnumerable<ITestCaseData> Event_TestCases()
        {
            var outgoing = GetSimpleEvent();
            var expected = GetSimpleEvent();
            yield return new TestCaseData(outgoing, expected).SetName("Events with start, end, and duration");

            var fiveA = GetSimpleRecurrencePattern();
            var fiveB = GetSimpleRecurrencePattern();

            outgoing = GetSimpleEvent();
            expected = GetSimpleEvent();
            outgoing.RecurrenceRules = new List<RecurrencePattern> { fiveA };
            expected.RecurrenceRules = new List<RecurrencePattern> { fiveB };
            yield return new TestCaseData(outgoing, expected).SetName("Events with start, end, duration, and one recurrence rule");
        }

        [Fact]
        public void Calendar_Tests()
        {
            var rruleA = new RecurrencePattern(FrequencyType.Daily, 1)
            {
                Count = 5
            };

            var e = new CalendarEvent
            {
                DtStart = new CalDateTime(_nowTime),
                DtEnd = new CalDateTime(_later),
                Duration = TimeSpan.FromHours(1),
                RecurrenceRules = new List<RecurrencePattern> { rruleA },
            };

            var actualCalendar = new Calendar();
            actualCalendar.Events.Add(e);

            //Work around referential equality...
            var rruleB = new RecurrencePattern(FrequencyType.Daily, 1)
            {
                Count = 5
            };

            var expectedCalendar = new Calendar();
            expectedCalendar.Events.Add(new CalendarEvent
            {
                DtStart = new CalDateTime(_nowTime),
                DtEnd = new CalDateTime(_later),
                Duration = TimeSpan.FromHours(1),
                RecurrenceRules = new List<RecurrencePattern> { rruleB },
            });

            Assert.Equal(actualCalendar.GetHashCode(), expectedCalendar.GetHashCode());
            Assert.True(actualCalendar.Equals(expectedCalendar));
        }

        [Fact, TestCaseSource(nameof(VTimeZone_TestCases))]
        public void VTimeZone_Tests(VTimeZone actual, VTimeZone expected)
        {
            Assert.Equal(actual.Url, expected.Url);
            Assert.Equal(actual.TzId, expected.TzId);
            Assert.Equal(actual, expected);
            Assert.Equal(actual.GetHashCode(), expected.GetHashCode());
        }

        public static IEnumerable<ITestCaseData> VTimeZone_TestCases()
        {
            const string nzSt = "New Zealand Standard Time";
            var first = new VTimeZone
            {
                TzId = nzSt,
            };
            var second = new VTimeZone(nzSt);
            yield return new TestCaseData(first, second);

            first.Url = new Uri("http://example.com/");
            second.Url = new Uri("http://example.com");
            yield return new TestCaseData(first, second);
        }

        [Fact, TestCaseSource(nameof(Attendees_TestCases))]
        public void Attendees_Tests(Attendee actual, Attendee expected)
        {
            Assert.Equal(expected.GetHashCode(), actual.GetHashCode());
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<ITestCaseData> Attendees_TestCases()
        {
            var tentative1 = new Attendee("MAILTO:james@example.com")
            {
                CommonName = "James Tentative",
                Role = ParticipationRole.RequiredParticipant,
                Rsvp = true,
                ParticipationStatus = EventParticipationStatus.Tentative
            };
            var tentative2 = new Attendee("MAILTO:james@example.com")
            {
                CommonName = "James Tentative",
                Role = ParticipationRole.RequiredParticipant,
                Rsvp = true,
                ParticipationStatus = EventParticipationStatus.Tentative
            };
            yield return new TestCaseData(tentative1, tentative2).SetName("Simple attendee test case");

            var complex1 = new Attendee("MAILTO:mary@example.com")
            {
                CommonName = "Mary Accepted",
                Rsvp = true,
                ParticipationStatus = EventParticipationStatus.Accepted,
                SentBy = new Uri("mailto:someone@example.com"),
                DirectoryEntry = new Uri("ldap://example.com:6666/o=eDABC Industries,c=3DUS??(cn=3DBMary Accepted)"),
                Type = "CuType",
                Members = new List<string> { "Group A", "Group B" },
                Role = ParticipationRole.Chair,
                DelegatedTo = new List<string> { "Peon A", "Peon B" },
                DelegatedFrom = new List<string> { "Bigwig A", "Bigwig B" }
            };
            var complex2 = new Attendee("MAILTO:mary@example.com")
            {
                CommonName = "Mary Accepted",
                Rsvp = true,
                ParticipationStatus = EventParticipationStatus.Accepted,
                SentBy = new Uri("mailto:someone@example.com"),
                DirectoryEntry = new Uri("ldap://example.com:6666/o=eDABC Industries,c=3DUS??(cn=3DBMary Accepted)"),
                Type = "CuType",
                Members = new List<string> { "Group A", "Group B" },
                Role = ParticipationRole.Chair,
                DelegatedTo = new List<string> { "Peon A", "Peon B" },
                DelegatedFrom = new List<string> { "Bigwig A", "Bigwig B" }
            };
            yield return new TestCaseData(complex1, complex2).SetName("Complex attendee test");
        }

        [Fact, TestCaseSource(nameof(CalendarCollection_TestCases))]
        public void CalendarCollection_Tests(string rawCalendar)
        {
            var a = Calendar.Load(IcsFiles.UsHolidays);
            var b = Calendar.Load(IcsFiles.UsHolidays);

            Assert.NotNull(a);
            Assert.NotNull(b);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.Equal(a, b);
        }

        public static IEnumerable<ITestCaseData> CalendarCollection_TestCases()
        {
            yield return new TestCaseData(IcsFiles.Google1).SetName("Google calendar test case");
            yield return new TestCaseData(IcsFiles.Parse1).SetName("Weird file parse test case");
            yield return new TestCaseData(IcsFiles.UsHolidays).SetName("US Holidays (quite large)");
        }

        [Fact]
        public void Resources_Tests()
        {
            var origContents = new[] { "Foo", "Bar" };
            var e = GetSimpleEvent();
            e.Resources = new List<string>(origContents);
            Assert.True(e.Resources.Count == 2);

            e.Resources.Add("Baz");
            Assert.True(e.Resources.Count == 3);
            var serialized = SerializeEvent(e);
            Assert.True(serialized.Contains("Baz"));

            e.Resources.Remove("Baz");
            Assert.True(e.Resources.Count == 2);
            serialized = SerializeEvent(e);
            Assert.False(serialized.Contains("Baz"));

            e.Resources.Add("Hello");
            Assert.True(e.Resources.Contains("Hello"));
            serialized = SerializeEvent(e);
            Assert.True(serialized.Contains("Hello"));

            e.Resources.Clear();
            e.Resources.AddRange(origContents);
            Assert.Equivalent(e.Resources, origContents);
            serialized = SerializeEvent(e);
            Assert.True(serialized.Contains("Foo"));
            Assert.True(serialized.Contains("Bar"));
            Assert.False(serialized.Contains("Baz"));
            Assert.False(serialized.Contains("Hello"));
        }

        internal static (byte[] original, byte[] copy) GetAttachments()
        {
            var payload = Encoding.UTF8.GetBytes("This is an attachment!");
            var payloadCopy = new byte[payload.Length];
            Array.Copy(payload, payloadCopy, payload.Length);
            return (payload, payloadCopy);
        }

        [Fact, TestCaseSource(nameof(RecurringComponentAttachment_TestCases))]
        public void RecurringComponentAttachmentTests(RecurringComponent noAttachment, RecurringComponent withAttachment)
        {
            var attachments = GetAttachments();

            Assert.NotEqual(noAttachment, withAttachment);
            Assert.NotEqual(noAttachment.GetHashCode(), withAttachment.GetHashCode());

            noAttachment.Attachments.Add(new Attachment(attachments.copy));

            Assert.Equal(noAttachment, withAttachment);
            Assert.Equal(noAttachment.GetHashCode(), withAttachment.GetHashCode());
        }

        public static IEnumerable<ITestCaseData> RecurringComponentAttachment_TestCases()
        {
            var attachments = GetAttachments();

            var journalNoAttach = new Journal { Start = new CalDateTime(_nowTime), Summary = "A summary!", Class = "Some class!" };
            var journalWithAttach = new Journal { Start = new CalDateTime(_nowTime), Summary = "A summary!", Class = "Some class!" };
            journalWithAttach.Attachments.Add(new Attachment(attachments.original));
            yield return new TestCaseData(journalNoAttach, journalWithAttach).SetName("Journal recurring component attachment");

            var todoNoAttach = new Todo { Start = new CalDateTime(_nowTime), Summary = "A summary!", Class = "Some class!" };
            var todoWithAttach = new Todo { Start = new CalDateTime(_nowTime), Summary = "A summary!", Class = "Some class!" };
            todoWithAttach.Attachments.Add(new Attachment(attachments.original));
            yield return new TestCaseData(todoNoAttach, todoWithAttach).SetName("Todo recurring component attachment");

            var eventNoAttach = GetSimpleEvent();
            var eventWithAttach = GetSimpleEvent();
            eventWithAttach.Attachments.Add(new Attachment(attachments.original));
            yield return new TestCaseData(eventNoAttach, eventWithAttach).SetName("Event recurring component attachment");
        }

        [Fact, TestCaseSource(nameof(PeriodTestCases))]
        public void PeriodTests(Period a, Period b)
        {
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.Equal(a, b);
        }

        public static IEnumerable<ITestCaseData> PeriodTestCases()
        {
            yield return new TestCaseData(new Period(new CalDateTime(_nowTime)), new Period(new CalDateTime(_nowTime)))
                .SetName("Two identical CalDateTimes are equal");
        }

        [Fact]
        public void PeriodListTests()
        {
            var startTimesA = new List<DateTime>
            {
                new DateTime(2017, 03, 02, 06, 00, 00),
                new DateTime(2017, 03, 03, 06, 00, 00),
                new DateTime(2017, 03, 06, 06, 00, 00),
                new DateTime(2017, 03, 07, 06, 00, 00),
                new DateTime(2017, 03, 08, 06, 00, 00),
                new DateTime(2017, 03, 09, 06, 00, 00),
                new DateTime(2017, 03, 10, 06, 00, 00),
                new DateTime(2017, 03, 13, 06, 00, 00),
                new DateTime(2017, 03, 14, 06, 00, 00),
                new DateTime(2017, 03, 17, 06, 00, 00),
                new DateTime(2017, 03, 20, 06, 00, 00),
                new DateTime(2017, 03, 21, 06, 00, 00),
                new DateTime(2017, 03, 22, 06, 00, 00),
                new DateTime(2017, 03, 23, 06, 00, 00),
                new DateTime(2017, 03, 24, 06, 00, 00),
                new DateTime(2017, 03, 27, 06, 00, 00),
                new DateTime(2017, 03, 28, 06, 00, 00),
                new DateTime(2017, 03, 29, 06, 00, 00),
                new DateTime(2017, 03, 30, 06, 00, 00),
                new DateTime(2017, 03, 31, 06, 00, 00),
                new DateTime(2017, 04, 03, 06, 00, 00),
                new DateTime(2017, 04, 05, 06, 00, 00),
                new DateTime(2017, 04, 06, 06, 00, 00),
                new DateTime(2017, 04, 07, 06, 00, 00),
                new DateTime(2017, 04, 10, 06, 00, 00),
                new DateTime(2017, 04, 11, 06, 00, 00),
                new DateTime(2017, 04, 12, 06, 00, 00),
                new DateTime(2017, 04, 13, 06, 00, 00),
                new DateTime(2017, 04, 17, 06, 00, 00),
                new DateTime(2017, 04, 18, 06, 00, 00),
                new DateTime(2017, 04, 19, 06, 00, 00),
                new DateTime(2017, 04, 20, 06, 00, 00),
                new DateTime(2017, 04, 21, 06, 00, 00),
                new DateTime(2017, 04, 24, 06, 00, 00),
                new DateTime(2017, 04, 25, 06, 00, 00),
                new DateTime(2017, 04, 27, 06, 00, 00),
                new DateTime(2017, 04, 28, 06, 00, 00),
                new DateTime(2017, 05, 01, 06, 00, 00),
            }
            .Select(dt => new Period(new CalDateTime(dt))).ToList();
            var a = new PeriodList();
            foreach (var period in startTimesA)
            {
                a.Add(period);
            }

            //Difference from A: first element became the second, and last element became the second-to-last element
            var startTimesB = new List<DateTime>
            {
                new DateTime(2017, 03, 03, 06, 00, 00),
                new DateTime(2017, 03, 02, 06, 00, 00),
                new DateTime(2017, 03, 06, 06, 00, 00),
                new DateTime(2017, 03, 07, 06, 00, 00),
                new DateTime(2017, 03, 08, 06, 00, 00),
                new DateTime(2017, 03, 09, 06, 00, 00),
                new DateTime(2017, 03, 10, 06, 00, 00),
                new DateTime(2017, 03, 13, 06, 00, 00),
                new DateTime(2017, 03, 14, 06, 00, 00),
                new DateTime(2017, 03, 17, 06, 00, 00),
                new DateTime(2017, 03, 20, 06, 00, 00),
                new DateTime(2017, 03, 21, 06, 00, 00),
                new DateTime(2017, 03, 22, 06, 00, 00),
                new DateTime(2017, 03, 23, 06, 00, 00),
                new DateTime(2017, 03, 24, 06, 00, 00),
                new DateTime(2017, 03, 27, 06, 00, 00),
                new DateTime(2017, 03, 28, 06, 00, 00),
                new DateTime(2017, 03, 29, 06, 00, 00),
                new DateTime(2017, 03, 30, 06, 00, 00),
                new DateTime(2017, 03, 31, 06, 00, 00),
                new DateTime(2017, 04, 03, 06, 00, 00),
                new DateTime(2017, 04, 05, 06, 00, 00),
                new DateTime(2017, 04, 06, 06, 00, 00),
                new DateTime(2017, 04, 07, 06, 00, 00),
                new DateTime(2017, 04, 10, 06, 00, 00),
                new DateTime(2017, 04, 11, 06, 00, 00),
                new DateTime(2017, 04, 12, 06, 00, 00),
                new DateTime(2017, 04, 13, 06, 00, 00),
                new DateTime(2017, 04, 17, 06, 00, 00),
                new DateTime(2017, 04, 18, 06, 00, 00),
                new DateTime(2017, 04, 19, 06, 00, 00),
                new DateTime(2017, 04, 20, 06, 00, 00),
                new DateTime(2017, 04, 21, 06, 00, 00),
                new DateTime(2017, 04, 24, 06, 00, 00),
                new DateTime(2017, 04, 25, 06, 00, 00),
                new DateTime(2017, 04, 27, 06, 00, 00),
                new DateTime(2017, 05, 01, 06, 00, 00),
                new DateTime(2017, 04, 28, 06, 00, 00),
            }
            .Select(dt => new Period(new CalDateTime(dt))).ToList();
            var b = new PeriodList();
            foreach (var period in startTimesB)
            {
                b.Add(period);
            }

            var collectionEqual = CollectionHelpers.Equals(a, b);
            Assert.Equal(true, collectionEqual);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());

            var listOfListA = new List<PeriodList> { a };
            var listOfListB = new List<PeriodList> { b };
            Assert.True(CollectionHelpers.Equals(listOfListA, listOfListB));

            var aThenB = new List<PeriodList> { a, b };
            var bThenA = new List<PeriodList> { b, a };
            Assert.True(CollectionHelpers.Equals(aThenB, bThenA));
        }

        [Fact]
        public void CalDateTimeTests()
        {
            var nowLocal = DateTime.Now;
            var nowUtc = nowLocal.ToUniversalTime();

            var asLocal = new CalDateTime(nowLocal, "America/New_York");
            var asUtc = new CalDateTime(nowUtc, "UTC");

            Assert.NotEqual(asLocal, asUtc);
        }

        private void TestComparison(Func<CalDateTime, IDateTime, bool> calOp, Func<int?, int?, bool> intOp)
        {
            int? intSome = 1;
            int? intGreater = 2;

            var dtSome = new CalDateTime(2018, 1, 1);
            var dtGreater = new CalDateTime(2019, 1, 1);

            Assert.Equal(intOp(null, null), calOp(null, null));
            Assert.Equal(intOp(null, intSome), calOp(null, dtSome));
            Assert.Equal(intOp(intSome, null), calOp(dtSome, null));
            Assert.Equal(intOp(intSome, intSome), calOp(dtSome, dtSome));
            Assert.Equal(intOp(intSome, intGreater), calOp(dtSome, dtGreater));
            Assert.Equal(intOp(intGreater, intSome), calOp(dtGreater, dtSome));
        }

        [Fact]
        public void CalDateTimeComparisonOperatorTests()
        {
            // Assumption: comparison operators on CalDateTime are expected to
            // work like operators on Nullable<int>.
            TestComparison((dt1, dt2) => dt1 == dt2, (i1, i2) => i1 == i2);
            TestComparison((dt1, dt2) => dt1 != dt2, (i1, i2) => i1 != i2);
            TestComparison((dt1, dt2) => dt1 > dt2, (i1, i2) => i1 > i2);
            TestComparison((dt1, dt2) => dt1 >= dt2, (i1, i2) => i1 >= i2);
            TestComparison((dt1, dt2) => dt1 < dt2, (i1, i2) => i1 < i2);
            TestComparison((dt1, dt2) => dt1 <= dt2, (i1, i2) => i1 <= i2);
        }
    }
}
