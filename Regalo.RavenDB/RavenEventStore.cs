using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Regalo.Core.EventSourcing;

namespace Regalo.RavenDB
{
    public class RavenEventStore : IEventStore
    {
        private readonly IDocumentStore _documentStore;

        public RavenEventStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Store(Guid aggregateId, object evt)
        {
            throw new NotImplementedException();
        }

        public void Store(Guid aggregateId, IEnumerable<object> events)
        {
            using (var session = _documentStore.OpenSession())
            {
                var aggregateIdAsString = aggregateId.ToString();

                foreach (var evt in events)
                {
                    session.Store(new EventContainer(aggregateIdAsString, evt));
                }

                session.SaveChanges();
            }
        }

        public IEnumerable<object> Load(Guid aggregateId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var aggregateIdAsString = aggregateId.ToString();

                var events = (from container in session.Query<EventContainer>()
                              where container.AggregateId == aggregateIdAsString
                              select container.Event).ToList();

                return events.Count > 0 ? events : null;
            }
        }

        public IEnumerable<object> Load(Guid aggregateId, int minVersion, int maxVersion)
        {
            using (var session = _documentStore.OpenSession())
            {
                var aggregateIdAsString = aggregateId.ToString();

                var events = (from container in session.Query<EventContainer>()
                              where container.AggregateId == aggregateIdAsString
                              select container.Event).Skip(minVersion - 1).Take(maxVersion - minVersion + 1).ToList();

                return events.Count > 0 ? events : null;
            }
        }
    }
}