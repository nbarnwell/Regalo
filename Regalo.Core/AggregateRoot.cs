using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public abstract class AggregateRoot
    {
        private readonly IDictionary<string, MethodInfo> _applyMethodCache = new Dictionary<string, MethodInfo>();
        private readonly IList<object> _uncommittedEvents = new List<object>();

        public Guid Id { get; protected set; }
        public Guid? BaseVersion { get; private set; }

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
            SetParentVersion(evt);

            ApplyEvent(evt);

            ValidateHasId();

            _uncommittedEvents.Add(evt);
        }

        private void SetParentVersion(object evt)
        {
            var versionHandler = Resolver.Resolve<IVersionHandler>();
            versionHandler.SetParentVersion(evt, FindCurrentVersion());
        }

        private void ApplyEvent(object evt)
        {
            var applyMethods = FindApplyMethods(evt);

            foreach (var applyMethod in applyMethods)
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

        private Guid? FindCurrentVersion()
        {
            if (_uncommittedEvents.Any())
            {
                var versionHandler = Resolver.Resolve<IVersionHandler>();
                return versionHandler.GetVersion(_uncommittedEvents.Last());
            }
            else
            {
                return BaseVersion;
            }
        }

        /// <summary>
        /// Returns the Apply methods for each type in the event type's inheritance
        /// hierarchy, top-down, from interfaces before classes.
        /// </summary>
        private IEnumerable<MethodInfo> FindApplyMethods(object evt)
        {
            var typeInspector = new TypeInspector();

            var applyMethods = typeInspector.GetTypeHierarchy(evt.GetType())
                                            .Select(FindApplyMethod)
                                            .Where(x => x != null)
                                            .ToList();

            return applyMethods;
        }

        private MethodInfo FindApplyMethod(Type eventType)
        {
            MethodInfo applyMethod;
            if (_applyMethodCache.Count == 0 || !_applyMethodCache.TryGetValue(eventType.Name, out applyMethod))
            {
                applyMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(m => m.Name == "Apply")
                    .Where(
                        m =>
                        {
                            var parameters = m.GetParameters();
                            return parameters.Length == 1 && parameters[0].ParameterType == eventType;
                        }).SingleOrDefault();

                _applyMethodCache.Add(eventType.Name, applyMethod);
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