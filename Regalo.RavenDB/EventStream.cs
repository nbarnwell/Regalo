using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Regalo.RavenDB
{
    public class EventStream
    {
        private List<object> _events;

        public EventStream(string id)
        {
            Id = id;
            _events = new List<object>();
        }

        public string Id { get; private set; }

        public IEnumerable<object> Events
        {
            get { return _events.ToArray(); }
            private set { _events = new List<object>(value); }
        }

        public void Append(IEnumerable<object> events)
        {
            _events.AddRange(events);
        }
    }
}