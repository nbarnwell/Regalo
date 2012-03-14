using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Regalo.Core;

namespace Regalo.EventSourcing.Raven
{
    public class RavenRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
    {
        private readonly IDocumentStore _documentStore;

        public RavenRepository(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public TAggregateRoot Get(string id)
        {
            using (var session = _documentStore.OpenSession())
            {
                var events = (from container in session.Query<EventContainer>()
                              where container.AggregateId == id
                              select container.Event).ToList();

                if (events.Count == 0) return null;

                var aggregateRoot = new TAggregateRoot();

                aggregateRoot.ApplyAll(events);

                return aggregateRoot;
            }
        }

        public void Save(TAggregateRoot item)
        {
            IEnumerable<object> events = item.GetUncommittedEvents();
            using (var session = _documentStore.OpenSession())
            {
                foreach (var evt in events)
                {
                    session.Store(new EventContainer(evt));
                }

                session.SaveChanges();
            }
        }
    }
}