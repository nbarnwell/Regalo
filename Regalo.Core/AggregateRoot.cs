using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public abstract class AggregateRoot
    {
        private readonly IDictionary<string, MethodInfo> _methodIndex = new Dictionary<string, MethodInfo>();
        private readonly IList<object> _uncommittedEvents = new List<object>();

        public Guid Id { get; protected set; }
        public Guid BaseVersion { get; private set; }

        public IEnumerable<object> GetUncommittedEvents()
        {
            return _uncommittedEvents.ToList();
        }

        public void AcceptUncommittedEvents()
        {
            if (_uncommittedEvents.Any() == false) return;

            var version = FindCurrentVersion();

            BaseVersion = version;
            _uncommittedEvents.Clear();
        }

        public void ApplyAll(IEnumerable<object> events)
        {
            object lastEvent = null;
            foreach (var evt in events)
            {
                ApplyEvent(evt);

                lastEvent = evt;
            }

            if (lastEvent != null)
            {
                var versionHandler = Resolver.Resolve<IVersionHandler>();
                var currentVersion = versionHandler.GetVersion(lastEvent);
                BaseVersion = currentVersion;
            }
        }

        protected void Record(object evt)
        {
            ApplyEvent(evt);

            ValidateHasId();

            var versionHandler = Resolver.Resolve<IVersionHandler>();
            versionHandler.Append(_uncommittedEvents, evt);
        }

        private void ApplyEvent(object evt)
        {
            var applyMethod = FindApplyMethod(evt);

            if (applyMethod != null)
            {
                applyMethod.Invoke(this, new[] { evt });
            }
        }

        private void ValidateHasId()
        {
            if (Id == default(Guid))
            {
                throw new IdNotSetException();
            }
        }

        private Guid FindCurrentVersion()
        {
            var versionHandler = Resolver.Resolve<IVersionHandler>();
            var version = versionHandler.GetCurrentVersion(_uncommittedEvents);
            return version;
        }

        private MethodInfo FindApplyMethod(object evt)
        {
            Type eventType = evt.GetType();
            MethodInfo applyMethod;
            if (_methodIndex.Count == 0 || !_methodIndex.TryGetValue(eventType.Name, out applyMethod))
            {
                applyMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(m => m.Name == "Apply")
                    .Where(
                        m =>
                        {
                            var parameters = m.GetParameters();
                            return parameters.Length == 1 && parameters[0].ParameterType == evt.GetType();
                        }).SingleOrDefault();

                _methodIndex.Add(eventType.Name, applyMethod);
            }

            if (Conventions.AggregatesMustImplementApplyMethods && applyMethod == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Class {0} does not implement Apply({1} evt). Either implement the method or set Conventions.AggregatesMustImplementApplyMethods to false.",
                        GetType().Name,
                        eventType.Name));
            }
           
            return applyMethod;
        }
    }
}