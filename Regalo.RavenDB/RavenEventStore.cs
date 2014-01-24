using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Regalo.Core;
using Regalo.Core.EventSourcing;

namespace Regalo.RavenDB
{
    public class RavenEventStore : IEventStore
    {
        private readonly IDocumentStore _documentStore;
        private readonly IVersionHandler _versionHandler;

        public RavenEventStore(IDocumentStore documentStore, IVersionHandler versionHandler)
        {
            if (documentStore == null) throw new ArgumentNullException("documentStore");
            if (versionHandler == null) throw new ArgumentNullException("versionHandler");

            _documentStore = documentStore;
            _versionHandler = versionHandler;
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

                    SetRavenCollectionName(events, session, stream);
                }
                else
                {
                    stream.Append(events);
                }

                session.SaveChanges();
            }
        }

        private static void SetRavenCollectionName(IEnumerable<object> events, IDocumentSession session, EventStream stream)
        {
            if (Conventions.FindAggregateTypeForEventType != null)
            {
                var aggregateType = Conventions.FindAggregateTypeForEventType(events.First().GetType());
                session.Advanced.GetMetadataFor(stream)[Constants.RavenEntityName] =
                    DocumentConvention.DefaultTypeTagName(aggregateType);
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

        public IEnumerable<object> Load(Guid aggregateId, Guid maxVersion)
        {
            var events = Load(aggregateId).ToList();

            if (!events.Any(x => _versionHandler.GetVersion(x) == maxVersion))
            {
                throw new ArgumentOutOfRangeException(
                    "maxVersion",
                    maxVersion,
                    string.Format("Version {0} not found for aggregate {1}", maxVersion, aggregateId));
            }

            return GetEventsForVersion(events, maxVersion).ToList();
        }

        private IEnumerable<object> GetEventsForVersion(IEnumerable<object> events, Guid maxVersion)
        {
            foreach (var evt in events)
            {
                yield return evt;

                var eventVersion = _versionHandler.GetVersion(evt);
                if (eventVersion == maxVersion) break;
            }
        }
    }
}