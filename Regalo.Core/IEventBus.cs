using System.Collections.Generic;

namespace Regalo.Core
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent evt);
        void Publish<TEvent>(IEnumerable<TEvent> events);
    }
}