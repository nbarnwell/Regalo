using System;
using System.Linq;
using NUnit.Framework;
using Regalo.Core.EventSourcing;
using Regalo.Core.Tests.Unit.DomainModel.Users;

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
            IRepository<User> repository = new EventSourcingRepository<User>(eventStore);
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
            IRepository<User> repository = new EventSourcingRepository<User>(eventStore);
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
            IRepository<User> repository = new EventSourcingRepository<User>(eventStore);

            // Act
            User user = repository.Get(userId);

            // Assert
            Assert.Throws<InvalidOperationException>(() => user.ChangePassword("newnewpassword")); // Should fail if the user's events were correctly applied
        }

        [Test]
        public void GivenNewAggregateRoot_WhenBehaviourIsInvokedAndEventsRaised_ThenAggregateVersionShouldIncrement()
        {
            // Arrange
            var user = new User();
            
            // Act
            user.Register();

            // Assert
            Assert.AreEqual(1, user.Version, "User version is not correct.");

            // Act
            user.ChangePassword("newpassword");

            // Assert
            Assert.AreEqual(2, user.Version, "User version is not correct.");
        }

        [Test]
        public void GivenPopulatedEventStore_WhenLoadingAggregate_ThenAggregateVersionShouldReflectStoredEvents()
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
            IRepository<User> repository = new EventSourcingRepository<User>(eventStore);

            // Act
            User user = repository.Get(userId);

            // Assert
            Assert.AreEqual(3, user.Version);
        }
    }
}