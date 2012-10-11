using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class AggregateTests : TestFixtureBase
    {
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
            CollectionAssertAreJsonEqual(expected, actual);
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
            CollectionAssertAreJsonEqual(expectedBefore, before);
            CollectionAssertAreJsonEqual(expectedAfter, after);
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
        public void ApplyingNoEvents_GivenNewAggregateObject_ShouldNotModifyState()
        {
            // Arrange
            var user = new User();

            // Act
            user.ApplyAll(Enumerable.Empty<object>());

            // Assert
            AssertAreJsonEqual(Guid.Empty, user.BaseVersion);
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
            var userId = Guid.Parse("{42B90234-926D-4AA6-A960-F610D52F8F88}");
            var user = new User();
            var events = new object[] {new UserRegistered(userId), new UserChangedPassword("newpassword"), new UserChangedPassword("newpassword") };
            
            // Act
            user.ApplyAll(events);

            // Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newpassword"), "Expected exception stating the new password must be different the the previous one, indicating that previous events have replayed successfully.");
        }

        [Test]
        public void InvokingBehaviourThatDoesntSetId_GivenNewObject_ShouldFail()
        {
            var user = new User();

            Assert.Throws<IdNotSetException>(() => user.ChangePassword("newpassword"));
        }

        [Test]
        public void InvokingBehaviourOnObjectWithNoIdThatDoesntSetTheId_ShouldFail()
        {
            var user = new User();
            var events = new object[] { /*new UserRegistered(user.Id), */new UserChangedPassword("newpassword"), new UserChangedPassword("newpassword") };
            user.ApplyAll(events);

            Assert.Throws<IdNotSetException>(() => user.ChangePassword("newnewpassword"));
        }
    }
}