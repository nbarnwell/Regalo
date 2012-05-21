using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public interface IEventBus
    {
        [Obsolete("Use Publish<TEvent>(TEvent) instead.", true)]
        void Send<TEvent>(TEvent evt);
        
        void Publish<TEvent>(TEvent evt);

        [Obsolete("Use Publish<TEvent>(IEnumerable<TEvent>) instead.", true)]
        void Send<TEvent>(IEnumerable<TEvent> events);
        
        void Publish<TEvent>(IEnumerable<TEvent> events);
    }
}