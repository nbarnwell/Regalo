using System;
using System.Collections.Generic;
using NUnit.Framework;
using Regalo.Core.Tests.Unit.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
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
            user.Register();

            // Act
            user.ChangePassword("newpassword");
            IEnumerable<object> actual = user.GetUncommittedEvents();
            IEnumerable<object> expected = new object[]
                                               {
                                                   new UserRegistered(user.Id),
                                                   new UserChangedPassword("newpassword")
                                               };
            
            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void AcceptingEvents_GivenAggregateWithUncommittedEvents_ShouldClearUncommittedEvents()
        {
            // Arrange
            var user = new User();
            user.Register();
            user.ChangePassword("newpassword");

            // Act
            IEnumerable<object> expectedBefore = new object[] { new UserRegistered(user.Id), new UserChangedPassword("newpassword") };
            IEnumerable<object> expectedAfter = new object[0];

            IEnumerable<object> before = user.GetUncommittedEvents();
            user.AcceptUncommittedEvents();
            IEnumerable<object> after = user.GetUncommittedEvents();
            
            // Assert
            CollectionAssert.AreEqual(expectedBefore, before);
            CollectionAssert.AreEqual(expectedAfter, after);
        }

        [Test]
        public void InvokingBehaviour_GivenAggregateWithInvariantLogic_ShouldFailIfInvariantIsNotSatisfied()
        {
            // Arrange
            var user = new User();
            user.Register();
            user.ChangePassword("newpassword");

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newpassword"), "Expected exception stating the new password must be different the the previous one.");
        }

        [Test]
        public void ApplyingPreviouslyGeneratedEvents_GivenNewAggregateObject_ShouldBringAggregateBackToPreviousState()
        {
            // Arrange
            var user = new User();
            user.Register();
            var events = new object[] {new UserRegistered(user.Id), new UserChangedPassword("newpassword"), new UserChangedPassword("newerpassword") };

            // Act
            user.ApplyAll(events);

            // Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newerpassword"), "Expected exception stating the new password must be different the the previous one, indicating that previous events have replayed successfully.");
        }

        [Test]
        public void ApplyingPreviousEvents_GivenEventsThatWouldNotSatisfyCurrentInvariantLogic_ShouldNotFail()
        {
            // Arrange
            var user = new User();
            var userId = Guid.NewGuid();
            var events = new object[] {new UserRegistered(user.Id), new UserChangedPassword("newpassword"), new UserChangedPassword("newpassword") };
            
            // Act
            user.ApplyAll(events);

            // Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newpassword"), "Expected exception stating the new password must be different the the previous one, indicating that previous events have replayed successfully.");
        }
    }
}