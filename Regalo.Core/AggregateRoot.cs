using System;
using System.Collections.Generic;
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
            return _uncommittedEvents;
        }

        public void AcceptUncommittedEvents()
        {
            var version = FindCurrentVersion();

            BaseVersion = version;
            _uncommittedEvents.Clear();
        }

        public void ApplyAll(IEnumerable<object> events)
        {
            foreach (var evt in events)
            {
                var eventType = evt.GetType();

                MethodInfo applyMethod;
                if (_methodIndex.Count == 0 || !_methodIndex.TryGetValue(eventType.Name, out applyMethod))
                {
                    // Find and cache the apply method, even if there isn't one (so we don't try looking again)
                    applyMethod = GetApplyMethod(evt);
                    _methodIndex.Add(eventType.Name, applyMethod);
                }

                if (Conventions.AggregatesMustImplementApplyMethods && applyMethod == null)
                {
                    throw new InvalidOperationException(string.Format("Class {0} does not implement Apply({1} evt)", GetType().Name, eventType.Name));
                }

                if (applyMethod != null)
                {
                    applyMethod.Invoke(this, new[] { evt });
                }
            }

            BaseVersion = FindCurrentVersion();
        }

        protected void Record(object evt)
        {
            var versionHandler = Resolver.Resolve<IVersionHandler>();
            versionHandler.Append(_uncommittedEvents, evt);
            
            var applyMethod = GetApplyMethod(evt);

            if (applyMethod != null)
            {
                applyMethod.Invoke(this, new[] { evt });
            }
        }

        private Guid FindCurrentVersion()
        {
            var versionHandler = Resolver.Resolve<IVersionHandler>();
            var version = versionHandler.FindCurrentVersion(_uncommittedEvents);
            return version;
        }

        private MethodInfo GetApplyMethod(object evt)
        {
            var applyMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name == "Apply")
                .Where(m =>
                           {
                               var parameters = m.GetParameters();
                               return parameters.Length == 1 && parameters[0].ParameterType == evt.GetType();
                           }).SingleOrDefault();
            return applyMethod;
        }
    }
}