using System;
using System.Reflection;
using Regalo.Core;

namespace Regalo.EventSourcing.Raven
{
    public class EventContainer
    {
        public string AggregateId { get; private set; }
        public object Event { get; private set; }

        public EventContainer(object evt)
        {
            var propertyInfo = evt.GetType().GetProperty(Conventions.AggregateIdPropertyName, BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null) throw new InvalidOperationException();

            Event = evt;
        }
    }
}