using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public abstract class MessageProcessorBase
    {
        private readonly ILogger _logger;
        private readonly IDictionary<RuntimeTypeHandle, MethodInfo> _handleMethodCache = new Dictionary<RuntimeTypeHandle, MethodInfo>();
        private readonly IDictionary<RuntimeTypeHandle, bool> _eventHandlingSuccessEventTypeCache = new Dictionary<RuntimeTypeHandle, bool>();

        protected MessageProcessorBase(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
        }

        protected void HandleMessage<TMessage>(TMessage message, Type messageHandlerOpenType)
        {
            var messageType = message.GetType();

            var targets = GetHandlerDescriptors(messageHandlerOpenType, messageType);

            // Throw if there are no handlers. Unless it's a successwrapper
            // event, for which it's not obligatory to have a handler.
            if (!IsEventHandlingSuccessEvent(message) && targets.IsEmpty())
            {
                throw new InvalidOperationException(string.Format("No handlers registered for: {0}", message));
            }

            foreach (var target in targets)
            {
                _logger.Debug(this, "Invoking {0} with {1}", target.Handler, message);
                target.MethodInfo.Invoke(target.Handler, new object[] { message });
            }
        }

        private bool IsEventHandlingSuccessEvent(object evt)
        {
            var eventType = evt.GetType();

            bool result;
            if (!_eventHandlingSuccessEventTypeCache.TryGetValue(eventType.TypeHandle, out result))
            {
                result = eventType.GetInterfaces()
                                  .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandlingSucceededEvent<>));
                _eventHandlingSuccessEventTypeCache.Add(eventType.TypeHandle, result);
            }

            return result;
        }

        private List<HandlerDescriptor> GetHandlerDescriptors(Type messageHandlerOpenType, Type messageType)
        {
            // TODO: Re-instate the IsEventHandlingResultEvent() method here
            var isEventHandlingResultEvent = IsEventHandlingSuccessEvent(messageType);

            var messageTypes = isEventHandlingResultEvent
                                   ? GetEventHandlingResultEventTypeHierarchy(messageType)
                                   : GetEventTypeHierarchy(messageType);

            var targets = messageTypes.Select(x => new { MessageType = x, HandlerType = messageHandlerOpenType.MakeGenericType(x) })
                                      .SelectMany(
                                          x => Resolver.ResolveAll(x.HandlerType),
                                          (x, handler) => new HandlerDescriptor
                                          {
                                              MethodInfo = FindHandleMethod(x.MessageType, x.HandlerType),
                                              Handler = handler
                                          })
                                      .ToList();
            return targets;
        }

        private static IEnumerable<Type> GetEventTypeHierarchy(Type eventType)
        {
            var inspector = new TypeInspector();
            return inspector.GetTypeHierarchy(eventType);
        }

        private static IEnumerable<Type> GetEventHandlingResultEventTypeHierarchy(Type type)
        {
            var expectedOpenGenericTypes = new[] { typeof(IEventHandlingSucceededEvent<>), typeof(IEventHandlingFailedEvent<>) };
            var closedGenericType = type.GetInterfaces().Where(i => i.IsGenericType)
                                                        .First(i => expectedOpenGenericTypes.Contains(i.GetGenericTypeDefinition()));
            var openGenericType = closedGenericType.GetGenericTypeDefinition();
            var eventType = closedGenericType.GetGenericArguments().Single();

            var eventTypes = GetEventTypeHierarchy(eventType);
            return eventTypes.Select(t => openGenericType.MakeGenericType(t));
        }

        private MethodInfo FindHandleMethod(Type messageType, Type handlerType)
        {
            MethodInfo handleMethod;
            if (false == _handleMethodCache.TryGetValue(handlerType.TypeHandle, out handleMethod))
            {
                handleMethod = handlerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                          .Where(m => m.Name == "Handle")
                                          .Where(
                                              m =>
                                              {
                                                  var parameters = m.GetParameters();
                                                  return parameters.Length == 1 && parameters[0].ParameterType == messageType;
                                              }).SingleOrDefault();

                _handleMethodCache.Add(handlerType.TypeHandle, handleMethod);
            }

            return handleMethod;
        }

        private class HandlerDescriptor
        {
            public MethodInfo MethodInfo { get; set; }
            public object Handler { get; set; }
        }
    }
}