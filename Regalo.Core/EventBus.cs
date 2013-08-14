using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public class EventBus : MessageProcessorBase, IEventBus
    {
        private readonly ILogger _logger;

        public EventBus(ILogger logger) 
            : base(logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
        }

        public void Publish<TEvent>(TEvent evt)
        {
            var eventType = evt.GetType();

            try
            {
                HandleMessage(evt, typeof(IEventHandler<>));
            }
            catch (Exception e)
            {
                if (eventType != typeof(object))
                {
                    var failedEvent = MakeFailureEvent(evt, e);
                    _logger.Error(this, e, "Failed to handle {0}, publishing {1}...", evt, failedEvent);
                    HandleMessage(failedEvent, typeof(IEventHandler<>));
                }
                else
                {
                    _logger.Error(this, e, "Failed to handle {0} but NOT publishing EventHandlingFailedEvent<object>...", evt);
                }
            }

            if (eventType != typeof(object))
            {
                var succeededEvent = MakeSuccessEvent(evt);
                _logger.Debug(this, "Handled {0}, publishing {1}...", evt, succeededEvent);
                HandleMessage(succeededEvent, typeof(IEventHandler<>));
            }
            else
            {
                _logger.Debug(this, "Handled {0} but NOT publishing EventHandlingSucceededEvent<object>...", evt);
            }
        }

        public void Publish<TEvent>(IEnumerable<TEvent> events)
        {
            foreach (var evt in events)
            {
                Publish(evt);
            }
        }

        private static object MakeSuccessEvent(object evt)
        {
            var eventType = evt.GetType();
            var wrapperEventType = typeof(EventHandlingSucceededEvent<>).MakeGenericType(eventType);
            var wrapperEvent = Activator.CreateInstance(wrapperEventType, evt);
            return wrapperEvent;
        }

        private static object MakeFailureEvent(object evt, Exception e)
        {
            var eventType = evt.GetType();
            var wrapperEventType = typeof(EventHandlingFailedEvent<>).MakeGenericType(eventType);
            var wrapperEvent = Activator.CreateInstance(wrapperEventType, evt, e);
            return wrapperEvent;
        }
    }
}