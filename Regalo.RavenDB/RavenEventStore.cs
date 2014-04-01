using System;
using System.Collections.Generic;
using Regalo.Core.EventSourcing;

namespace Regalo.RavenDB
{
    [Obsolete("Use DelayedWriteRavenEventStore instead, for improved performance from Preload and save characteristics.", true)]
    public class RavenEventStore : IEventStore
    {
        public void Add(Guid aggregateId, IEnumerable<object> events)
        {
            throw new NotImplementedException();
        }

        public void Update(Guid aggregateId, IEnumerable<object> events)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> Load(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> Load(Guid aggregateId, Guid maxVersion)
        {
            throw new NotImplementedException();
        }
    }
}