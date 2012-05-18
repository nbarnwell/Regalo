using System;
using System.Collections.Generic;
using System.Linq;

namespace Regalo.Core.EventSourcing
{
    public class EventSourcingRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;
        private readonly IConcurrencyMonitor _concurrencyMonitor;

        public EventSourcingRepository(IEventStore eventStore, IConcurrencyMonitor concurrencyMonitor)
        {
            _eventStore = eventStore;
            _concurrencyMonitor = concurrencyMonitor;
        }

        public TAggregateRoot Get(Guid id)
        {
            object[] events = _eventStore.Load(id).ToArray();

            if (events.Length == 0) return null;

            var aggregateRoot = new TAggregateRoot();

            aggregateRoot.ApplyAll(events);

            return aggregateRoot;
        }

        public void Save(TAggregateRoot item)
        {
            var uncommittedEvents = item.GetUncommittedEvents().ToArray();

            if (uncommittedEvents.Length == 0) return;

            object[] baseAndUnseenEvents = _eventStore.Load(item.Id).ToArray();

            if (baseAndUnseenEvents.Length > 0)
            {
                object[] baseEvents = baseAndUnseenEvents.Take(item.BaseVersion).ToArray();
                object[] unseenEvents = baseAndUnseenEvents.Skip(item.BaseVersion).ToArray();
                _concurrencyMonitor.CheckForConflicts(baseEvents, unseenEvents, uncommittedEvents);
            }

            _eventStore.Store(item.Id, uncommittedEvents);
            item.AcceptUncommittedEvents();
        }
    }
}