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
            PropertyInfo propertyInfo = evt.GetType().GetProperty(Conventions.AggregateIdPropertyName, BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null) throw new InvalidOperationException(string.Format("No public instance property found with name according to Aggregate Id property name convention of {0}", Conventions.AggregateIdPropertyName));

            AggregateId = (string)propertyInfo.GetValue(evt, null);
            Event = evt;
        }
    }
}