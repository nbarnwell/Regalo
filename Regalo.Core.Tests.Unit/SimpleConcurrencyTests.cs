using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Regalo.Core.EventSourcing;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class SimpleConcurrencyTests
    {
        [Test]
        public void GivenNoUnseenEventsAndNoUncommittedEvents_WhenTestingForConflicts_ThenFindNoConflicts()
        {
            // Arrange
            IConcurrencyMonitor monitor = new StrictConcurrencyMonitor();
            IEnumerable<object> unseenEvents = Enumerable.Empty<object>();
            IEnumerable<object> uncommittedEvents = Enumerable.Empty<object>();

            // Act
            IEnumerable<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(unseenEvents, uncommittedEvents);

            // Assert
            CollectionAssert.IsEmpty(conflicts, "Conflicts have been returned where there should be none.");
        }
        
        [Test]
        public void GivenNoUnseenEvents_WhenTestingForConflicts_ThenFindNoConflicts()
        {
            // Arrange
            IConcurrencyMonitor monitor = new StrictConcurrencyMonitor();
            IEnumerable<object> unseenEvents = Enumerable.Empty<object>();
            IEnumerable<object> uncommittedEvents = new[]
                                                        {
                                                            new UserRegistered(Guid.NewGuid())
                                                        };

            // Act
            IEnumerable<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(unseenEvents, uncommittedEvents);

            // Assert
            CollectionAssert.IsEmpty(conflicts, "Conflicts have been returned where there should be none.");
        }

        [Test]
        public void GivenUnseenEventsAndNoUncommittedEvents_WhenTestingForConflicts_ThenFindNoConflicts()
        {
            // Arrange
            IConcurrencyMonitor monitor = new StrictConcurrencyMonitor();
            IEnumerable<object> unseenEvents = new[] { new UserRegistered(Guid.NewGuid()) };
            IEnumerable<object> uncommittedEvents = Enumerable.Empty<object>();

            // Act
            IEnumerable<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(unseenEvents, uncommittedEvents);

            // Assert
            CollectionAssert.IsEmpty(conflicts, "Conflicts have been returned where there should be none.");
        }

        [Test]
        public void GivenUnseenEvents_WhenTestingForConflicts_ThenFindConflicts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            IConcurrencyMonitor monitor = new StrictConcurrencyMonitor();
            IEnumerable<object> unseenEvents = new[] { new UserChangedPassword("newpassword") };
            IEnumerable<object> uncommittedEvents = new[] { new UserChangedPassword("differentnewpassword") };

            // Act
            IList<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(unseenEvents, uncommittedEvents).ToList();

            // Assert
            Assert.AreEqual(1, conflicts.Count, "Expected one and only one conflict.");
            var conflict = conflicts.Single();
            CollectionAssert.AreEqual(unseenEvents, conflict.UnseenEvents);
            CollectionAssert.AreEqual(uncommittedEvents, conflict.UncommittedEvents);
            Assert.AreEqual("Changes conflict with one or more committed events.", conflict.Message);
        }
    }
}