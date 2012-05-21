using System.Collections.Generic;
using Regalo.Core;

namespace Regalo.Testing
{
    public class FakeEventBus : IEventBus
    {
        public void Send<TEvent>(TEvent evt)
        {
            throw new System.NotImplementedException();
        }

        public void Send<TEvent>(IEnumerable<TEvent> evt)
        {
            throw new System.NotImplementedException();
        }
    }
}