using System;

namespace Regalo.RavenDB
{
    public class EventContainer
    {
        public string AggregateId { get; private set; }

        public object Event { get; private set; }

        public EventContainer(string aggregateId, object evt)
        {
            AggregateId = aggregateId;
            Event = evt;
        }
    }
}