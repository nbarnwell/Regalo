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
            return Get(id, null);
        }

        public TAggregateRoot Get(Guid id, Guid version)
        {
            return Get(id, (Guid?)version);
        }

        private TAggregateRoot Get(Guid id, Guid? version)
        {
            var events = version == null
                ? _eventStore.Load(id)
                : _eventStore.Load(id, version.Value)
                ;

            events = events.ToList();

            if (!events.Any()) return null;

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
                var unseenEvents = GetUnseenEvents(item, baseAndUnseenEvents);
                _concurrencyMonitor.CheckForConflicts(unseenEvents, uncommittedEvents);
            }

            _eventStore.Store(item.Id, uncommittedEvents);
            item.AcceptUncommittedEvents();
        }

        private static IEnumerable<object> GetUnseenEvents(TAggregateRoot item, IList<object> baseAndUnseenEvents)
        {
            var versionHandler = Resolver.Resolve<IVersionHandler>();
            var unseenEvents = new Stack<object>();

            // Reverse through the list, adding items to the "unseen" events list
            // until we find the "base" event, put all previous events into the base
            // events list in one go.
            for (int i = baseAndUnseenEvents.Count - 1; i >= 0; i--)
            {
                var evt = baseAndUnseenEvents[i];

                if (versionHandler.GetVersion(evt) != item.BaseVersion)
                {
                    unseenEvents.Push(evt);
                }
                else
                {
                    break;
                }
            }

            return unseenEvents;
        }
    }
}