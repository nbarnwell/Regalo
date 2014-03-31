using System;

namespace Regalo.Core
{
    public static class EventHandlingSucceededEvent
    {
        public static IEventHandlingSucceededEvent<TEvent> Create<TEvent>(TEvent evt)
        {
            return (IEventHandlingSucceededEvent<TEvent>)WrapEvent(evt);
        }

        private static object WrapEvent(object evt)
        {
            var wrapperType = typeof(EventHandlingSucceededEventImpl<>).MakeGenericType(evt.GetType());
            return Activator.CreateInstance(wrapperType, evt);
        }

        private class EventHandlingSucceededEventImpl<TEvent> : EventHandlingResultEvent, IEventHandlingSucceededEvent<TEvent>
        {
            public TEvent Evt { get; private set; }

            public EventHandlingSucceededEventImpl(TEvent evt)
            {
                Evt = evt;
            }
        }
    }
}