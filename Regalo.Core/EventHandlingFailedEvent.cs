using System;

namespace Regalo.Core
{
    public class EventHandlingFailedEvent<TEvent> : EventHandlingResultEvent
    {
        public TEvent    Evt       { get; private set; }
        public Exception Exception { get; private set; }

        public EventHandlingFailedEvent(TEvent evt, Exception exception)
        {
            Evt       = evt;
            Exception = exception;
        }
    }
}