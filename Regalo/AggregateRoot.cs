using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo
{
    public abstract class AggregateRoot
    {
        private IList<Event> _uncommittedEvents = new List<Event>(); 

        public string Id { get; set; }
        
        protected void Record(Event evt)
        {
            _uncommittedEvents.Add(evt);

            var applyMethod = GetApplyMethod(evt);

            if (applyMethod != null)
            {
                applyMethod.Invoke(this, new object[] { evt });
            }
        }

        public IEnumerable<Event> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        public void AcceptUncommittedEvents()
        {
            _uncommittedEvents = new List<Event>();
        }

        private MethodInfo GetApplyMethod(Event evt)
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

        public void ApplyAll(IEnumerable<Event> events)
        {
            var methodIndex = new Dictionary<string, MethodInfo>();

            foreach (var evt in events)
            {
                var eventType = evt.GetType();

                MethodInfo applyMethod;
                if (methodIndex.Count == 0 || !methodIndex.TryGetValue(eventType.Name, out applyMethod))
                {
                    // Find and cache the apply method, even if there isn't one (so we don't try looking again)
                    applyMethod = GetApplyMethod(evt);
                    methodIndex.Add(eventType.Name, applyMethod);
                }

                if (applyMethod != null)
                {
                    applyMethod.Invoke(this, new object[] { evt });
                }
            }
        }
    }
}