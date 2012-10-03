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
            Store(aggregateId, new[] { evt });
        }

        public void Store(Guid aggregateId, IEnumerable<object> events)
        {
            using (var session = _documentStore.OpenSession())
            {
                var aggregateIdAsString = aggregateId.ToString();

                var stream = session.Load<EventStream>(aggregateIdAsString);

                if (stream == null)
                {
                    stream = new EventStream(aggregateIdAsString);
                    stream.Append(events);
                    session.Store(stream);
                }
                else
                {
                    stream.Append(events);
                }

                session.SaveChanges();
            }
        }

        public IEnumerable<object> Load(Guid aggregateId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var aggregateIdAsString = aggregateId.ToString();

                var stream = session.Load<EventStream>(aggregateIdAsString);

                if (stream != null)
                {
                    var events = stream.Events;

                    return events.Any() ? events : Enumerable.Empty<object>();
                }

                return Enumerable.Empty<object>();
            }
        }

        public IEnumerable<object> Load(Guid aggregateId, int minVersion, int maxVersion)
        {
            return Load(aggregateId)
                .Skip(minVersion - 1)
                .Take(maxVersion - (minVersion - 1));
        }
    }
}