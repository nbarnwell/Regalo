using System;
using System.Collections.Generic;
using System.Linq;

namespace Regalo.Core
{
    public class RuntimeConfiguredVersionHandler : IVersionHandler
    {
        private readonly IDictionary<Type, Func<object, Guid>> _getVersionDelegateCache = new Dictionary<Type, Func<object, Guid>>();
        private readonly IDictionary<Type, Action<object>> _setParentVersionDelegateCache = new Dictionary<Type, Action<object>>();

        public void Append(IList<object> events, object evt)
        {
            var eventType = evt.GetType();

            Action<object> setParentVersionHandler;
            if (!_setParentVersionDelegateCache.TryGetValue(eventType, out setParentVersionHandler))
            {
                throw new InvalidOperationException(string.Format("You have not configured a method for setting the Parent Version for events of type {0}", eventType));
            }

            setParentVersionHandler.Invoke(evt);
        }

        public Guid GetCurrentVersion(IEnumerable<object> events)
        {
            var lastEvent = events.LastOrDefault();
            
            if (lastEvent == null) throw new ArgumentException("An empty event list was supplied.");

            return GetVersion(lastEvent);
        }

        public Guid GetVersion(object evt)
        {
            var eventType = evt.GetType();

            Func<object, Guid> getVersionHandler;
            if (!_getVersionDelegateCache.TryGetValue(eventType, out getVersionHandler))
            {
                throw new InvalidOperationException(string.Format("You have not configured a method for getting the Version for events of type {0}", eventType));
            }

            return getVersionHandler.Invoke(evt);
        }

        public void AddConfiguration(Type type, Func<object, Guid> getVersionHandler, Action<object> setParentVersionHandler)
        {
            _getVersionDelegateCache.Add(type, getVersionHandler);
            _setParentVersionDelegateCache.Add(type, setParentVersionHandler);
        }
    }
}