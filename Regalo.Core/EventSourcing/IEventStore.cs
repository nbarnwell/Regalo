using System;
using System.Collections.Generic;

namespace Regalo.Core.EventSourcing
{
    public interface IEventStore
    {
        void Add(Guid aggregateId, IEnumerable<object> events);
        void Update(Guid aggregateId, IEnumerable<object> events);
        IEnumerable<object> Load(Guid aggregateId);
        IEnumerable<object> Load(Guid aggregateId, Guid maxVersion);
    }
}