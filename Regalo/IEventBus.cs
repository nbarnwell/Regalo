using System.Collections.Generic;

namespace Regalo
{
    public interface IEventBus
    {
        void Send<TEvent>(TEvent evt) where TEvent : Event;
        void Send<TEvent>(IEnumerable<TEvent> evt) where TEvent : Event;
    }
}