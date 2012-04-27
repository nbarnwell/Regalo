using System;
using System.Collections.Generic;

namespace Regalo.Core.EventSourcing
{
    public interface IEventStore
    {
        void Store(Guid aggregateId, object evt);
        void Store(Guid aggregateId, IEnumerable<object> events);
        IEnumerable<object> Load(Guid aggregateId);
        IEnumerable<object> Load(Guid aggregateId, int minVersion, int maxVersion);
    }
}