using System;
using System.Collections.Generic;
using System.Linq;

namespace Regalo.Core.EventSourcing
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IDictionary<Guid, IList<object>> _aggregates = new Dictionary<Guid, IList<object>>();

        public IEnumerable<object> Load(Guid aggregateId)
        {
            return FindAggregateEventList(aggregateId);
        }

        public IEnumerable<object> Load(Guid aggregateId, int minVersion, int maxVersion)
        {
            IList<object> events = FindAggregateEventList(aggregateId);
            return events != null ? events.Skip(minVersion - 1).Take(maxVersion - minVersion + 1) : null;
        }

        public void Store(Guid aggregateId, object evt)
        {
            IList<object> aggregateEventList = GetAggregateEventList(aggregateId);

            aggregateEventList.Add(evt);
        }

        public void Store(Guid aggregateId, IEnumerable<object> events)
        {
            IList<object> aggregateEventList = GetAggregateEventList(aggregateId);

            foreach (var evt in events)
            {
                aggregateEventList.Add(evt);
            }
        }

        private IList<object> FindAggregateEventList(Guid aggregateId)
        {
            IList<object> aggregateEventList;
            if (!_aggregates.TryGetValue(aggregateId, out aggregateEventList))
            {
                return null;
            }

            return aggregateEventList;
        }

        private IList<object> GetAggregateEventList(Guid aggregateId)
        {
            IList<object> aggregateEventList = FindAggregateEventList(aggregateId);
            if (aggregateEventList == null)
            {
                aggregateEventList = new List<object>();
                _aggregates.Add(aggregateId, aggregateEventList);
            }
            return aggregateEventList;
        }

        public IEnumerable<object> Events
        {
            get { return _aggregates.Values.SelectMany(list => list); }
        }
    }
}