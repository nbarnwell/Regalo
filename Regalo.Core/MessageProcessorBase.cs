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

        protected MessageProcessorBase(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
        }

        protected void HandleMessage<TMessage>(TMessage message, Type messageHandlerOpenType)
        {
            var inspector = new TypeInspector();
            var targets = inspector.GetTypeHierarchy(message.GetType())
                                    .Select(x => new { MessageType = x, HandlerType = messageHandlerOpenType.MakeGenericType(x) })
                                    .SelectMany(
                                        x => Resolver.ResolveAll(x.HandlerType),
                                        (x, handler) => new
                                        {
                                            MethodInfo = FindHandleMethod(x.MessageType, x.HandlerType),
                                            Handler = handler
                                        })
                                    .ToList();

            if (false == targets.Any())
            {
                throw new InvalidOperationException(string.Format("No handlers registered for: {0}", message));
            }

            foreach (var target in targets)
            {
                _logger.Debug(this, "Invoking {0} with {1}", target.Handler, message);
                target.MethodInfo.Invoke(target.Handler, new object[] { message });
            }
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
    }
}