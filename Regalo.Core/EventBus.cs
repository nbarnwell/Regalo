using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public class EventBus : MessageProcessorBase, IEventBus
    {
        public EventBus(ILogger logger) 
            : base(logger)
        {
        }

        public void Publish<TEvent>(TEvent evt)
        {
            HandleMessage(evt, typeof(IEventHandler<>));
        }

        public void Publish<TEvent>(IEnumerable<TEvent> events)
        {
            foreach (var evt in events)
            {
                Publish(evt);
            }
        }
    }
}