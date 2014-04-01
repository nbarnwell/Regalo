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
    public class DelayedWriteRavenEventStore : IDelayedWriteEventStore, IEventStoreWithPreloading, IDisposable
    {
        private readonly IVersionHandler _versionHandler;

        private IDocumentSession _documentSession;

        public DelayedWriteRavenEventStore(IDocumentStore documentStore, IVersionHandler versionHandler)
        {
            if (documentStore == null) throw new ArgumentNullException("documentStore");
            if (versionHandler == null) throw new ArgumentNullException("versionHandler");

            _documentSession = documentStore.OpenSession();
            _versionHandler = versionHandler;
        }

        public void Add(Guid aggregateId, IEnumerable<object> events)
        {
            var stream = new EventStream(aggregateId.ToString());
            stream.Append(events);
            _documentSession.Store(stream);

            SetRavenCollectionName(events, _documentSession, stream);
        }

        public void Update(Guid aggregateId, IEnumerable<object> events)
        {
            var aggregateIdAsString = aggregateId.ToString();

            var stream = _documentSession.Load<EventStream>(aggregateIdAsString);

            if (stream == null)
            {
                throw new InvalidOperationException("You cannot update an aggregate that has not been saved.");
            }

            stream.Append(events);
        }

        public void Flush()
        {
            _documentSession.SaveChanges();
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

        public void Preload(IEnumerable<Guid> aggregateIds)
        {
            var idValues = aggregateIds.Select(x => x.ToString());
            _documentSession.Load<EventStream>(idValues);
        }

        public IEnumerable<object> Load(Guid aggregateId)
        {
            var aggregateIdAsString = aggregateId.ToString();

            var stream = _documentSession.Load<EventStream>(aggregateIdAsString);

            if (stream != null)
            {
                var events = stream.Events;

                return events.Any() ? events : Enumerable.Empty<object>();
            }

            return Enumerable.Empty<object>();
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

        public void Dispose()
        {
            if (_documentSession != null)
            {
                if (_documentSession.Advanced.HasChanges)
                {
                    throw new InvalidOperationException("Disposing a delayed-write event store with pending changes. Be sure to call Flush() when all operations are completed.");
                }

                _documentSession.Dispose();
                _documentSession = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}