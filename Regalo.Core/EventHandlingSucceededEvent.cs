namespace Regalo.Core
{
    public class EventHandlingSucceededEvent<TEvent> : EventHandlingResultEvent
    {
        public TEvent Evt { get; private set; }

        public EventHandlingSucceededEvent(TEvent evt)
        {
            Evt = evt;
        }
    }
}