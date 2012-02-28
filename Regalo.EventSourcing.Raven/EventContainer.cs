using Regalo.Core;

namespace Regalo.EventSourcing.Raven
{
    public class EventContainer
    {
        public Event Event { get; set; }

        public EventContainer(Event evt)
        {
            Event = evt;
        }

    }
}