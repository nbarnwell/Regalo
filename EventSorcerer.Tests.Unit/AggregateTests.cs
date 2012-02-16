using System;
using System.Collections.Generic;
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

        [Test]
        public void AcceptingEvents_GivenAggregateWithUncommittedEvents_ShouldClearUncommittedEvents()
        {
            // Arrange
            var user = new User();
            user.Id = Guid.NewGuid().ToString();
            user.ChangePassword("newpassword");

            // Act
            IEnumerable<Event> expectedBefore = new Event[] { new UserChangedPassword(user.Id, "newpassword") };
            IEnumerable<Event> expectedAfter = new Event[0];

            IEnumerable<Event> before = user.GetUncommittedEvents();
            user.AcceptUncommittedEvents();
            IEnumerable<Event> after = user.GetUncommittedEvents();
            
            // Assert
            CollectionAssert.AreEqual(expectedBefore, before);
            CollectionAssert.AreEqual(expectedAfter, after);
        }

        [Test]
        public void InvokingBehaviour_GivenAggregateWithInvariantLogic_ShouldFailIfInvariantIsNotSatisfied()
        {
            // Arrange
            var user = new User();
            user.Id = Guid.NewGuid().ToString();
            user.ChangePassword("newpassword");

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newpassword"), "Expected exception stating the new password must be different the the previous one.");
        }

        [Test]
        public void ApplyingPreviouslyGeneratedEvents_GivenNewAggregateObject_ShouldBringAggregateBackToPreviousState()
        {
            // Arrange
            var user = new User();
            user.Id = Guid.NewGuid().ToString();
            var events = new Event[] { new UserChangedPassword(user.Id, "newpassword"), new UserChangedPassword(user.Id, "newerpassword") };

            // Act
            user.ApplyAll(events);

            // Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newerpassword"), "Expected exception stating the new password must be different the the previous one, indicating that previous events have replayed successfully.");
        }
    }

    // ReSharper restore InconsistentNaming

    

    public class UserChangedPassword : Event
    {
        public string UserId { get; private set; }
        public string NewPassword { get; private set; }

        public UserChangedPassword(string userId, string newpassword)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            if (newpassword == null) throw new ArgumentNullException("newpassword");

            UserId = userId;
            NewPassword = newpassword;
        }

        public bool Equals(UserChangedPassword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.UserId, UserId) && Equals(other.NewPassword, NewPassword);
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
                return ((UserId != null ? UserId.GetHashCode() : 0) * 397) ^ (NewPassword != null ? NewPassword.GetHashCode() : 0);
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

    public class User : AggregateRoot
    {
        private string _password;

        public void ChangePassword(string newpassword)
        {
            if (string.IsNullOrWhiteSpace(newpassword)) throw new InvalidOperationException("New password cannot be empty or whitespace.");
            if (newpassword == _password) throw new InvalidOperationException("New password cannot be the same as the old password.");
         
            Record(new UserChangedPassword(Id, newpassword));
        }

        private void Apply(UserChangedPassword evt)
        {
            _password = evt.NewPassword;
        }
    }
}