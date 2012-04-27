using System;
using System.Collections.Generic;

namespace Regalo.Core.EventSourcing
{
    public class EventSourcingRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;

        public EventSourcingRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TAggregateRoot Get(Guid id)
        {
            IEnumerable<object> events = _eventStore.Load(id);

            if (events == null) return null;

            var aggregateRoot = new TAggregateRoot();

            aggregateRoot.ApplyAll(events);

            return aggregateRoot;
        }

        public void Save(TAggregateRoot item)
        {
            IEnumerable<object> events = item.GetUncommittedEvents();

            // Concurrency control/event conflict merging

            _eventStore.Store(item.Id, events);
        }
    }
}