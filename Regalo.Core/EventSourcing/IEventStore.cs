using System;
using System.Collections.Generic;

namespace Regalo.Core.EventSourcing
{
    public interface IEventStore
    {
        IEnumerable<object> Load(Guid aggregateId);
        void Store(Guid aggregateId, object evt);
        void Store(Guid aggregateId, IEnumerable<object> events);
        IEnumerable<object> Events { get; }
    }
}