using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class AggregateTests
    {
        private void CollectionAssertAreJsonEqual(IEnumerable<object> expected, IEnumerable<object> actual)
        {
            var expectedJson = expected.Select(x => JsonConvert.SerializeObject(x, Formatting.Indented));
            var actualJson   = actual.Select(x => JsonConvert.SerializeObject(x, Formatting.Indented));

            CollectionAssert.AreEqual(expectedJson, actualJson);
        }

        private void AssertAreJsonEqual(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
            var actualJson   = JsonConvert.SerializeObject(actual, Formatting.Indented);

            Assert.AreEqual(expectedJson, actualJson);
        }

        [SetUp]
        public void SetUp()
        {
            var versionHandler = new RuntimeConfiguredVersionHandler();
            versionHandler.AddConfiguration<UserChangedPassword>(e => e.Version, (e, v) => e.Version = v);
            versionHandler.AddConfiguration<UserRegistered>(e => e.Version, (e, v) => e.Version = v);

            Resolver.SetResolver(
                t =>
                {
                    if (t == typeof(IVersionHandler)) return versionHandler;

                    throw new NotSupportedException(string.Format("Nothing registered in SetUp for {0}", t));
                });
        }

        [TearDown]
        public void TearDown()
        {
            Resolver.ClearResolver();
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