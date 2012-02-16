using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace EventSorcerer.Tests.Unit
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class AggregateTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void InvokingBehaviour_GivenSimpleAggregateRoot_ShouldRecordEvents()
        {
            // Arrange
            var user = new User();
            user.Id = Guid.NewGuid().ToString();

            // Act
            user.ChangePassword("newpassword");
            IEnumerable<Event> actual = user.GetUncommittedEvents();
            IEnumerable<Event> expected = new Event[] { new UserChangedPassword(user.Id, "newpassword") };
            
            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }

    // ReSharper restore InconsistentNaming

    

    public class UserChangedPassword : Event
    {
        public string UserId { get; private set; }
        public string Newpassword { get; private set; }

        public UserChangedPassword(string userId, string newpassword)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            if (newpassword == null) throw new ArgumentNullException("newpassword");

            UserId = userId;
            Newpassword = newpassword;
        }

        public bool Equals(UserChangedPassword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.UserId, UserId) && Equals(other.Newpassword, Newpassword);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(UserChangedPassword)) return false;
            return Equals((UserChangedPassword)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UserId != null ? UserId.GetHashCode() : 0) * 397) ^ (Newpassword != null ? Newpassword.GetHashCode() : 0);
            }
        }

        public static bool operator ==(UserChangedPassword left, UserChangedPassword right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserChangedPassword left, UserChangedPassword right)
        {
            return !Equals(left, right);
        }
    }

    public abstract class Event : Message
    {
    }

    public abstract class Message
    {
    }

    public class User : AggregateRoot
    {
        public void ChangePassword(string newpassword)
        {
            if (string.IsNullOrWhiteSpace(newpassword)) throw new InvalidOperationException("New password cannot be empty or whitespace.");
         
            Record(new UserChangedPassword(Id, newpassword));
        }
    }

    public abstract class AggregateRoot
    {
        private readonly IList<Event> _uncommittedEvents = new List<Event>(); 

        public string Id { get; set; }
        
        protected void Record(Event evt)
        {
            _uncommittedEvents.Add(evt);
        }

        public IEnumerable<Event> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }
    }
}