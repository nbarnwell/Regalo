using System.Collections.Generic;

namespace Regalo.Core
{
    public interface IEventStore
    {
        IEnumerable<Event> Load(string aggregateId);
        void Store(IEnumerable<Event> events);
    }
}