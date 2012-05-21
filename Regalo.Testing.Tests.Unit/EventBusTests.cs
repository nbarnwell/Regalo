using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Regalo.Core;

namespace Regalo.Testing.Tests.Unit
{
    [TestFixture]
    public class EventBusTests
    {
        [Test]
        public void GivenNoPreviousState_WhenPublishingSingleEvent_ThenEventsShouldBeStored()
        {
            // Arrange
            var eventBus = new FakeEventBus();

            // Act
            eventBus.Publish(new SomethingHappened());

            // Assert
            CollectionAssert.AreEqual(new object[] {new SomethingHappened()}, eventBus.Events);
        }

        [Test]
        public void GivenNoPreviousState_WhenPublishingEvents_ThenEventsShouldBeStored()
        {
            // Arrange
            var eventBus = new FakeEventBus();

            // Act
            eventBus.Publish((IEnumerable<object>)(new object[] { new SomethingHappened(), new SomethingElseHappened() }));

            // Assert
            CollectionAssert.AreEqual(new object[] { new SomethingHappened(), new SomethingElseHappened() }, eventBus.Events.ToArray());
        }
    }

    public class SomethingHappened {
        public bool Equals(SomethingHappened other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(SomethingHappened)) return false;
            return Equals((SomethingHappened)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(SomethingHappened left, SomethingHappened right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SomethingHappened left, SomethingHappened right)
        {
            return !Equals(left, right);
        }
    }

    public class SomethingElseHappened {
        public bool Equals(SomethingElseHappened other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(SomethingElseHappened)) return false;
            return Equals((SomethingElseHappened)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(SomethingElseHappened left, SomethingElseHappened right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SomethingElseHappened left, SomethingElseHappened right)
        {
            return !Equals(left, right);
        }
    }
}