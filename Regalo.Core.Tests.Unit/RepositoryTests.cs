using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Regalo.Core.EventSourcing;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class RepositoryTests
    {
        [Test]
        public void GivenAggregateWithNoUncommittedEvents_WhenSaved_ThenEventStoreShouldContainNoAdditionalEvents()
        {
            // Arrange
            var eventStore = new InMemoryEventStore();
            var repository = new EventSourcingRepository<User>(eventStore, new Mock<IConcurrencyMonitor>().Object);
            var user = new User();

            var expectedEvents = Enumerable.Empty<object>();

            // Act
            repository.Save(user);

            // Assert
            CollectionAssert.AreEqual(expectedEvents, eventStore.Events);
        }

        [Test]
        public void GivenAggregateWithUncommittedEvents_WhenSaved_ThenEventStoreShouldContainThoseEvents()
        {
            // Arrange
            var eventStore = new InMemoryEventStore();
            IRepository<User> repository = new EventSourcingRepository<User>(eventStore, new Mock<IConcurrencyMonitor>().Object);
            var user = new User();
            user.Register();

            var expectedEvents = new object[] { new UserRegistered(user.Id) };

            // Act
            repository.Save(user);

            // Assert
            CollectionAssert.AreEqual(expectedEvents, eventStore.Events);
        }

        [Test]
        public void GivenPopulatedEventStore_WhenLoadingAggregate_ThenRepositoryShouldRebuildThatAggregate()
        {
            // Arrange
            var eventStore = new InMemoryEventStore();
            var userId = Guid.NewGuid();
            eventStore.Store(userId, new object[]
                                         {
                                             new UserRegistered(userId),
                                             new UserChangedPassword("newpassword"),
                                             new UserChangedPassword("newnewpassword")
                                         });
            var repository = new EventSourcingRepository<User>(eventStore, new Mock<IConcurrencyMonitor>().Object);

            // Act
            User user = repository.Get(userId);

            // Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newnewpassword")); // Should fail if the user's events were correctly applied
        }

        [Test]
        public void GivenAnyAggregateRoot_WhenBehaviourIsInvokedAndEventsRaised_ThenBaseVersionShouldNotChange()
        {
            // Arrange
            var user = new User();
            
            // Act
            user.Register();

            // Assert
            Assert.AreEqual(0, user.BaseVersion, "User's base version is not correct.");

            // Act
            user.ChangePassword("newpassword");

            // Assert
            Assert.AreEqual(0, user.BaseVersion, "User's base version is not correct.");
        }

        [Test]
        public void GivenPopulatedEventStore_WhenLoadingAggregate_ThenAggregateVersionShouldReflectStoredEvents()
        {
            // Arrange
            var eventStore = new InMemoryEventStore();
            var userId = Guid.NewGuid();
            eventStore.Store(
                userId,
                new object[]
                {
                    new UserRegistered(userId),
                    new UserChangedPassword("newpassword"),
                    new UserChangedPassword("newnewpassword")
                });
            var repository = new EventSourcingRepository<User>(eventStore, new Mock<IConcurrencyMonitor>().Object);

            // Act
            User user = repository.Get(userId);

            // Assert
            Assert.AreEqual(3, user.BaseVersion);
        }

        [Test]
        public void GivenNewAggreateWithNoEvents_WhenSaving_ThenShouldNotBotherCheckingConcurrency()
        {
            // Arrange
            var user = new User();
            var concurrencyMonitor = new Mock<IConcurrencyMonitor>();
            var repository = new EventSourcingRepository<User>(new InMemoryEventStore(), concurrencyMonitor.Object);

            // Act
            repository.Save(user);

            // Assert
            concurrencyMonitor.Verify(monitor => monitor.CheckForConflicts(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>()), Times.Never());
        }

        [Test]
        public void GivenNewAggreateWithNewEvents_WhenSaving_ThenShouldNotBotherCheckingConcurrency()
        {
            // Arrange
            var user = new User();
            user.Register();
            var concurrencyMonitor = new Mock<IConcurrencyMonitor>();
            var repository = new EventSourcingRepository<User>(new InMemoryEventStore(), concurrencyMonitor.Object);

            // Act
            repository.Save(user);

            // Assert
            concurrencyMonitor.Verify(monitor => monitor.CheckForConflicts(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>()), Times.Never());
        }

        [Test]
        public void GivenExistingAggregateWithUnseenChanges_WhenSaving_ThenShouldCheckConcurrencyWithCorrectEvents()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventStore = new InMemoryEventStore();
            eventStore.Store(userId, new UserRegistered(userId));

            var concurrencyMonitor = new Mock<IConcurrencyMonitor>();
            var repository = new EventSourcingRepository<User>(eventStore, concurrencyMonitor.Object);
            var user = repository.Get(userId);

            // Now another user changes the password before we get chance to save our changes:
            eventStore.Store(userId, new UserChangedPassword("adifferentpassword"));

            user.ChangePassword("newpassword");

            // Act
            repository.Save(user);

            // Assert
            concurrencyMonitor.Verify(
                monitor =>
                monitor.CheckForConflicts(
                    It.Is<IEnumerable<object>>(
                        unseenEvents => (unseenEvents.Single() as UserChangedPassword).NewPassword == "adifferentpassword"),
                    It.Is<IEnumerable<object>>(
                        uncommittedEvents => (uncommittedEvents.Single() as UserChangedPassword).NewPassword == "newpassword")));
        }

        [Test]
        public void GivenAggregateWithUncommittedEvents_WhenSaved_ThenBaseVersionShouldMatchCurrentVersion()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventStore = new InMemoryEventStore();
            var userRegistered = new UserRegistered(userId);
            eventStore.Store(userId, userRegistered);

            var repository = new EventSourcingRepository<User>(eventStore, new Mock<IConcurrencyMonitor>().Object);
            var user = repository.Get(userId);
            user.ChangePassword("newpassword");

            var currentVersion = user.GetUncommittedEvents().Cast<Event>().Last().Version;

            // Act
            repository.Save(user);

            // Assert
            Assert.AreEqual(currentVersion, user.BaseVersion, "User's base version has not been updated to match current version on successful save.");
        }

        [Test]
        public void GivenAggregateWithUncommittedEvents_WhenSaving_ThenUncommittedEventsShouldBeAccepted()
        {
            // Arrange
            var user = new User();
            user.Register();
            var repository = new EventSourcingRepository<User>(new InMemoryEventStore(), new Mock<IConcurrencyMonitor>().Object);

            // Act
            repository.Save(user);

            // Assert
            CollectionAssert.IsEmpty(user.GetUncommittedEvents());
        }

        //Need new test for variations of conventions
    }
}