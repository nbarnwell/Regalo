using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Regalo.Core.EventSourcing;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class InMemoryEventStoreTests : TestFixtureBase
    {
        [Test]
        public void GivenEmptyEventStore_WhenLoadingEvents_ThenNothingShouldBeReturned()
        {
            // Arrange
            IEventStore store = new InMemoryEventStore();
            
            // Act
            IEnumerable<object> events = store.Load(Guid.NewGuid());

            // Assert
            CollectionAssert.IsEmpty(events);
        }

        [Test]
        public void GivenEmptyEventStore_WhenLoadingEventsForSpecificVersion_ThenNothingShouldBeReturned()
        {
            // Arrange
            IEventStore store = new InMemoryEventStore();

            // Act
            IEnumerable<object> events = store.Load(Guid.NewGuid(), 1, 3);

            // Assert
            CollectionAssert.IsEmpty(events);
        }

        [Test]
        public void GivenEmptyEventStore_WhenAddingEventsOneAtATime_ThenStoreShouldContainThoseEvents()
        {
            // Arrange
            IEventStore store = new InMemoryEventStore();
            var userId = Guid.NewGuid();
            var userRegistered = new UserRegistered(userId);

            // Act
            store.Store(userId, userRegistered);

            // Assert
            CollectionAssert.IsNotEmpty(((InMemoryEventStore)store).Events);
            Assert.AreSame(userRegistered, ((InMemoryEventStore)store).Events.First());
        }

        [Test]
        public void GivenEmptyEventStore_WhenAddingEventsInBulk_ThenStoreShouldContainThoseEvents()
        {
            // Arrange
            IEventStore store = new InMemoryEventStore();
            var user = new User();
            user.Register();
            user.ChangePassword("newpassword");

            // Act
            store.Store(user.Id, user.GetUncommittedEvents());

            // Assert
            CollectionAssert.IsNotEmpty(((InMemoryEventStore)store).Events);
            CollectionAssert.AreEqual(user.GetUncommittedEvents(), ((InMemoryEventStore)store).Events);
        }

        [Test]
        public void GivenEventStorePopulatedWithEventsForMultipleAggregates_WhenLoadingEventsForAnAggregate_ThenShouldReturnEventsForThatAggregate()
        {
            // Arrange
            IEventStore store = new InMemoryEventStore();

            var user1 = new User();
            user1.Register();
            user1.ChangePassword("user1pwd1");
            user1.ChangePassword("user1pwd2");

            var user2 = new User();
            user2.Register();
            user2.ChangePassword("user2pwd1");
            user2.ChangePassword("user2pwd2");

            store.Store(user1.Id, user1.GetUncommittedEvents());
            store.Store(user2.Id, user2.GetUncommittedEvents());

            // Act
            IEnumerable<object> eventsForUser1 = store.Load(user1.Id);

            // Assert
            CollectionAssert.AreEqual(user1.GetUncommittedEvents(), eventsForUser1, "Store didn't return user1's events properly.");
        }

        [Test]
        public void GivenEventStorePopulatedWithManyEventsForAnAggregate_WhenLoadingForSpecificVersion_ThenShouldOnlyLoadEventsUpToAndIncludingThatVersion()
        {
            // Arrange
            IEventStore store = new InMemoryEventStore();
            var id = Guid.NewGuid();
            var allEvents = new object[]
                                {
                                    new UserRegistered(id), // v1
                                    new UserChangedPassword("pwd1"), // v2
                                    new UserChangedPassword("pwd2"), // v3
                                    new UserChangedPassword("pwd3"), // v4
                                    new UserChangedPassword("pwd4"), // v5
                                };
            store.Store(id, allEvents);

            // Act
            IEnumerable<object> version3 = store.Load(id, 1, 3);

            // Assert
            CollectionAssert.AreEqual(allEvents.Take(3), version3);
        }
    }
}