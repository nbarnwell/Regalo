using System;

namespace Regalo.Core
{
    public static class EventHandlingFailedEvent
    {
        public static IEventHandlingFailedEvent<TEvent> Create<TEvent>(TEvent evt, Exception exception)
        {
            return (IEventHandlingFailedEvent<TEvent>)WrapEvent(evt, exception);
        }

        private static object WrapEvent(object evt, Exception exception)
        {
            var wrapperType = typeof(EventHandlingFailedEventImpl<>).MakeGenericType(evt.GetType());
            return Activator.CreateInstance(wrapperType, evt, exception);
        }

        private class EventHandlingFailedEventImpl<TEvent> : EventHandlingResultEvent, IEventHandlingFailedEvent<TEvent>
        {
            public TEvent Evt { get; private set; }
            public Exception Exception { get; private set; }

            public EventHandlingFailedEventImpl(TEvent evt, Exception exception)
            {
                Evt = evt;
                Exception = exception;
            }
        }
    }
}