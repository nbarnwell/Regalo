using System.Collections.Generic;

namespace Regalo.Core
{
    public interface IEventBus
    {
        void Send<TEvent>(TEvent evt);
        void Send<TEvent>(IEnumerable<TEvent> evt);
    }
}