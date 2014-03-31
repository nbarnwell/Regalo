using System;
using System.Collections.Generic;

namespace Regalo.Core.EventSourcing
{
    public interface IEventStoreWithPreloading : IEventStore
    {
        void Preload(IEnumerable<Guid> aggregateIds);
    }
}