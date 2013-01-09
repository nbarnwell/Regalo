using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public class MessageProcessorBase
    {
        private readonly IDictionary<RuntimeTypeHandle, MethodInfo> _handleMethodCache = new Dictionary<RuntimeTypeHandle, MethodInfo>();

        protected void HandleMessage<TMessage>(TMessage message, Type messageHandlerOpenType)
        {
            var inspector = new TypeInspector();
            var handlers = inspector.GetTypeHierarchy(message.GetType())
                                    .Select(x => new { MessageType = x, HandlerType = messageHandlerOpenType.MakeGenericType(x) })
                                    .Select(x => new { MethodInfo = FindHandleMethod(x.MessageType, x.HandlerType), Handler = Resolver.Resolve(x.HandlerType) })
                                    .ToList();

            if (false == handlers.Any())
            {
                throw new InvalidOperationException(string.Format("No handlers registered for: {0}", message));
            }

            foreach (var handler in handlers)
            {
                handler.MethodInfo.Invoke(handler.Handler, new object[] { message });
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