using System.Collections.Generic;
using System.Linq;

namespace Regalo.Core
{
    public class EventStoreRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;

        public EventStoreRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TAggregateRoot Get(string id)
        {
            IEnumerable<Event> events = _eventStore.Load(id);

            if (events == null || events.Any() == false) return null;

            var aggregateRoot = new TAggregateRoot();
            aggregateRoot.ApplyAll(events);
            return aggregateRoot;
        }

        public void Save(TAggregateRoot item)
        {
            IEnumerable<Event> events = item.GetUncommittedEvents();
            _eventStore.Store(events);
        }
    }
}