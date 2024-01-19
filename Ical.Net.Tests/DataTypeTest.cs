using Ical.Net.DataTypes;
using System.ComponentModel;
using Xunit;

namespace Ical.Net.Tests
{
    public class DataTypeTest
    {
        [Fact, Category("DataType")]
        public void OrganizerConstructorMustAcceptNull()
        {
            Assert.DoesNotThrow(() => { var o = new Organizer(null); });
        }

        [Fact, Category("DataType")]
        public void AttachmentConstructorMustAcceptNull()
        {
            Assert.DoesNotThrow(() => { var o = new Attachment((byte[])null); });
            Assert.DoesNotThrow(() => { var o = new Attachment((string)null); });
        }
    }
}