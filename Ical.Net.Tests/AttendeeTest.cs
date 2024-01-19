using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace Ical.Net.Tests
{
    public class AttendeeTest
    {
        internal static CalendarEvent VEventFactory() => new CalendarEvent
        {
            Summary = "Testing",
            Start = new CalDateTime(2010, 3, 25),
            End = new CalDateTime(2010, 3, 26)
        };

        private static readonly IList<Attendee> _attendees = new List<Attendee>
        {
            new Attendee("MAILTO:james@example.com")
            {
                CommonName = "James James",
                Role = ParticipationRole.RequiredParticipant,
                Rsvp = true,
                ParticipationStatus = EventParticipationStatus.Tentative
            },
            new Attendee("MAILTO:mary@example.com")
            {
                CommonName = "Mary Mary",
                Role = ParticipationRole.RequiredParticipant,
                Rsvp = true,
                ParticipationStatus = EventParticipationStatus.Accepted
            }
        }.AsReadOnly();


        /// <summary>
        /// Ensures that attendees can be properly added to an event.
        /// </summary>
        [Fact, Category("Attendee")]
        public void Add1Attendee()
        {
            var evt = VEventFactory();
            Assert.Equal(0, evt.Attendees.Count);

            evt.Attendees.Add(_attendees[0]);
            Assert.Equal(1, evt.Attendees.Count);

            //the properties below had been set to null during the Attendees.Add operation in NuGet version 2.1.4
            Assert.Equal(ParticipationRole.RequiredParticipant, evt.Attendees[0].Role);
            Assert.Equal(EventParticipationStatus.Tentative, evt.Attendees[0].ParticipationStatus);
        }

        [Fact, Category("Attendee")]
        public void Add2Attendees()
        {
            var evt = VEventFactory();
            Assert.Equal(0, evt.Attendees.Count);

            evt.Attendees.Add(_attendees[0]);
            evt.Attendees.Add(_attendees[1]);
            Assert.Equal(2, evt.Attendees.Count);
            Assert.Equal(ParticipationRole.RequiredParticipant, evt.Attendees[1].Role);

            var cal = new Calendar();
            cal.Events.Add(evt);
            var serializer = new CalendarSerializer();
            Console.Write(serializer.SerializeToString(cal));
        }

        /// <summary>
        /// Ensures that attendees can be properly removed from an event.
        /// </summary>
        [Fact, Category("Attendee")]
        public void Remove1Attendee()
        {
            var evt = VEventFactory();
            Assert.Equal(0, evt.Attendees.Count);

            var attendee = _attendees.First();
            evt.Attendees.Add(attendee);
            Assert.Equal(1, evt.Attendees.Count);

            evt.Attendees.Remove(attendee);
            Assert.Equal(0, evt.Attendees.Count);

            evt.Attendees.Remove(_attendees.Last());
            Assert.Equal(0, evt.Attendees.Count);
        }
    }
}
