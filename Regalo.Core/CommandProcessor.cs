using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regalo.Core
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IDictionary<RuntimeTypeHandle, MethodInfo> _handleMethodCache = new Dictionary<RuntimeTypeHandle, MethodInfo>();
        
        public void Process<TCommand>(TCommand command)
        {
            var inspector = new TypeInspector();
            var handlers = inspector.GetTypeHierarchy(command.GetType())
                                    .Select(x => new { CommandType = x, HandlerType = typeof(ICommandHandler<>).MakeGenericType(x) })
                                    .Select(x => new { MethodInfo = FindHandleMethod(x.CommandType, x.HandlerType), CommandHandler = Resolver.Resolve(x.HandlerType)})
                                    .ToList();

            if (false == handlers.Any()) throw new InvalidOperationException(string.Format("No handlers registered for command: {0}", command));

            foreach (var handler in handlers)
            {
                handler.MethodInfo.Invoke(handler.CommandHandler, new object[] { command });
            }
        }

        private MethodInfo FindHandleMethod(Type commandType, Type handlerType)
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
                            return parameters.Length == 1 && parameters[0].ParameterType == commandType;
                        }).SingleOrDefault();

                _handleMethodCache.Add(handlerType.TypeHandle, handleMethod);
            }

            return handleMethod;
        }
    }
}