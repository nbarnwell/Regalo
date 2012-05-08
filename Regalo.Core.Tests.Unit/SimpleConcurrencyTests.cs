using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Regalo.Core.Tests.Unit.DomainModel.Users;

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
            IEnumerable<object> baseEvents = Enumerable.Empty<object>();
            IEnumerable<object> unseenEvents = Enumerable.Empty<object>();
            IEnumerable<object> uncommittedEvents = Enumerable.Empty<object>();

            // Act
            IEnumerable<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(baseEvents, unseenEvents, uncommittedEvents);

            // Assert
            CollectionAssert.IsEmpty(conflicts, "Conflicts have been returned where there should be none.");
        }
        
        [Test]
        public void GivenNoUnseenEvents_WhenTestingForConflicts_ThenFindNoConflicts()
        {
            // Arrange
            IConcurrencyMonitor monitor = new StrictConcurrencyMonitor();
            IEnumerable<object> baseEvents = Enumerable.Empty<object>();
            IEnumerable<object> unseenEvents = Enumerable.Empty<object>();
            IEnumerable<object> uncommittedEvents = new[]
                                                        {
                                                            new UserRegistered(Guid.NewGuid())
                                                        };

            // Act
            IEnumerable<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(baseEvents, unseenEvents, uncommittedEvents);

            // Assert
            CollectionAssert.IsEmpty(conflicts, "Conflicts have been returned where there should be none.");
        }

        [Test]
        public void GivenUnseenEventsAndNoUncommittedEvents_WhenTestingForConflicts_ThenFindNoConflicts()
        {
            // Arrange
            IConcurrencyMonitor monitor = new StrictConcurrencyMonitor();
            IEnumerable<object> baseEvents = Enumerable.Empty<object>();
            IEnumerable<object> unseenEvents = new[] { new UserRegistered(Guid.NewGuid()) };
            IEnumerable<object> uncommittedEvents = Enumerable.Empty<object>();

            // Act
            IEnumerable<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(baseEvents, unseenEvents, uncommittedEvents);

            // Assert
            CollectionAssert.IsEmpty(conflicts, "Conflicts have been returned where there should be none.");
        }


        [Test]
        public void GivenUnseenEvents_WhenTestingForConflicts_ThenFindConflicts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            IConcurrencyMonitor monitor = new StrictConcurrencyMonitor();
            IEnumerable<object> baseEvents = new[] { new UserRegistered(userId) };
            IEnumerable<object> unseenEvents = new[] { new UserChangedPassword("newpassword") };
            IEnumerable<object> uncommittedEvents = new[] { new UserChangedPassword("differentnewpassword") };

            // Act
            IList<ConcurrencyConflict> conflicts = monitor.CheckForConflicts(baseEvents, unseenEvents, uncommittedEvents).ToList();

            // Assert
            Assert.AreEqual(1, conflicts.Count, "Expected one and only one conflict.");
            var conflict = conflicts.Single();
            CollectionAssert.AreEqual(baseEvents, conflict.BaseEvents);
            CollectionAssert.AreEqual(unseenEvents, conflict.UnseenEvents);
            CollectionAssert.AreEqual(uncommittedEvents, conflict.UncommittedEvents);
            Assert.AreEqual("Changes conflict with one or more committed events.", conflict.Message);
        }
    }

    public class ConcurrencyConflict
    {
        public IEnumerable BaseEvents { get; private set; }
        public IEnumerable UnseenEvents { get; private set; }
        public IEnumerable UncommittedEvents { get; private set; }
        public string Message { get; private set; }

        public ConcurrencyConflict(string message, IEnumerable baseEvents, IEnumerable unseenEvents, IEnumerable uncommittedEvents)
        {
            BaseEvents = baseEvents;
            UnseenEvents = unseenEvents;
            UncommittedEvents = uncommittedEvents;
            Message = message;
        }
    }

    public class StrictConcurrencyMonitor : IConcurrencyMonitor
    {
        public IEnumerable<ConcurrencyConflict> CheckForConflicts(IEnumerable<object> baseEvents, IEnumerable<object> unseenEvents, IEnumerable<object> uncommittedEvents)
        {
            if (baseEvents == null) throw new ArgumentNullException("baseEvents");
            if (unseenEvents == null) throw new ArgumentNullException("unseenEvents");
            if (uncommittedEvents == null) throw new ArgumentNullException("uncommittedEvents");

            if (unseenEvents.Any() && uncommittedEvents.Any())
            {
                return new[] { new ConcurrencyConflict("Changes conflict with one or more committed events.", baseEvents, unseenEvents, uncommittedEvents) };
            }

            return Enumerable.Empty<ConcurrencyConflict>();
        }
    }

    public interface IConcurrencyMonitor
    {
        IEnumerable<ConcurrencyConflict> CheckForConflicts(IEnumerable<object> baseEvents, IEnumerable<object> unseenEvents, IEnumerable<object> uncommittedEvents);
    }
}