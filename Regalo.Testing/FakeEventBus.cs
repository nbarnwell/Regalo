using System.Collections;
using System.Collections.Generic;
using Regalo.Core;

namespace Regalo.Testing
{
    public class FakeEventBus : IEventBus
    {
        private readonly IList<object> _events = new List<object>();

        public IEnumerable<object> Events { get { return _events; } }

        void IEventBus.Send<TEvent>(TEvent evt)
        {
            Publish(evt);
        }

        void IEventBus.Send<TEvent>(IEnumerable<TEvent> evt)
        {
            Publish(evt);
        }

        public void Publish<TEvent>(TEvent evt)
        {
            _events.Add(evt);
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