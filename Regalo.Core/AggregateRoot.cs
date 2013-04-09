using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public abstract class AggregateRoot
    {
        private static readonly ILogger __logger = Resolver.Resolve<ILogger>();
        
        private readonly IDictionary<RuntimeTypeHandle, MethodInfo> _applyMethodCache = new Dictionary<RuntimeTypeHandle, MethodInfo>();
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
            int eventCount = _uncommittedEvents.Count;
            _uncommittedEvents.Clear();
            __logger.Debug(this, "Accepted {0} uncommitted events. Now at base version {1}", eventCount, BaseVersion);
        }

        public void ApplyAll(IEnumerable<object> events)
        {
            object lastEvent = null;
            int i = 0;
            foreach (var evt in events)
            {
                ApplyEvent(evt);

                lastEvent = evt;
                i++;
            }

            if (lastEvent != null)
            {
                var versionHandler = Resolver.Resolve<IVersionHandler>();
                var currentVersion = versionHandler.GetVersion(lastEvent);
                BaseVersion = currentVersion;
            }

            __logger.Debug(this, "Applied {0} events. Now at base version {1}", i, BaseVersion);
        }

        protected void Record(object evt)
        {
            SetParentVersion(evt);

            ApplyEvent(evt);

            ValidateHasId();

            _uncommittedEvents.Add(evt);

            __logger.Debug(this, "Recorded new event: {0}", evt);
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
            if (false == _applyMethodCache.TryGetValue(eventType.TypeHandle, out applyMethod))
            {
                applyMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(m => m.Name == "Apply")
                    .Where(
                        m =>
                        {
                            var parameters = m.GetParameters();
                            return parameters.Length == 1 && parameters[0].ParameterType == eventType;
                        }).SingleOrDefault();

                _applyMethodCache.Add(eventType.TypeHandle, applyMethod);
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