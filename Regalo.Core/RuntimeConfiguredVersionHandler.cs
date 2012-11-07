using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public class RuntimeConfiguredVersionHandler : IVersionHandler
    {
        private readonly IDictionary<RuntimeTypeHandle, Func<object, Guid>> _versionSelectors = new Dictionary<RuntimeTypeHandle, Func<object, Guid>>();
        private readonly IDictionary<RuntimeTypeHandle, Action<object, Guid?>> _parentVersionSetters = new Dictionary<RuntimeTypeHandle, Action<object, Guid?>>();

        public Guid GetVersion(object evt)
        {
            var eventType = evt.GetType();

            Func<object, Guid> getVersionHandler;
            if (!_versionSelectors.TryGetValue(eventType.TypeHandle, out getVersionHandler))
            {
                throw new InvalidOperationException(string.Format("You have not configured a method for getting the Version for events of type {0}", eventType));
            }

            return getVersionHandler.Invoke(evt);
        }

        public void SetParentVersion(object evt, Guid? parentVersion)
        {
            var eventType = evt.GetType();

            Action<object, Guid?> setParentVersionHandler;
            if (!_parentVersionSetters.TryGetValue(eventType.TypeHandle, out setParentVersionHandler))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "You have not configured a method for setting the Parent Version for events of type {0}",
                        eventType));
            }

            setParentVersionHandler.Invoke(evt, parentVersion);
        }

        public void AddConfiguration<TEvent>(Func<TEvent, Guid> versionSelector, Action<TEvent, Guid?> parentVersionSetter)
        {
            _versionSelectors.Add(typeof(TEvent).TypeHandle, x => versionSelector.Invoke((TEvent)x));
            _parentVersionSetters.Add(typeof(TEvent).TypeHandle, (e, v) => parentVersionSetter.Invoke((TEvent)e, v));
        }
    }
}