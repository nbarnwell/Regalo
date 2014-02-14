using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Regalo.Core;
using Regalo.ObjectCompare;

namespace Regalo.Testing
{
    public class ScenarioAssert<TEntity, THandler, TCommand> : IScenarioAssert<TEntity, THandler, TCommand> 
        where TEntity : AggregateRoot, new()
    {
        private readonly TEntity _entity;
        private readonly THandler _handler;
        private readonly TCommand _command;
        private readonly TestingMessageHandlerContext<TEntity> _context;
        private readonly object[] _expected;

        public ScenarioAssert(TEntity entity, THandler handler, TCommand command, TestingMessageHandlerContext<TEntity> context, object[] expected)
        {
            if (context == null) throw new ArgumentNullException("context");

            _entity = entity;
            _handler = handler;
            _command = command;
            _context = context;
            _expected = expected;
        }

        public void Assert()
        {
            /*
             * Plan is to invoke the command on the handler and retrieve the actual
             * events from the repository and eventbus, and compare both with the
             * expected events list passed-in using Regalo.Object compare.
             */

            InvokeHandler();

            var eventsStoredToEventStore = _context.GetGeneratedEvents();

            var comparer = new ObjectComparer().Ignore<Event, Guid?>(x => x.ParentVersion)
                                               .Ignore<Event, Guid>(x => x.Version)
                                               .Ignore<Event, Guid>(x => x.Id);

            ObjectComparisonResult result = comparer.AreEqual(_expected, eventsStoredToEventStore);
            if (!result.AreEqual)
            {
                throw new AssertionException(string.Format("Actual events did not match expected events. {0}", result.InequalityReason));
            }
        }

        private static string FormatPropertyPath(IEnumerable<string> propertyChainDescription)
        {
            return string.Join(".", propertyChainDescription.Reverse());
        }

        private void InvokeHandler()
        {
            Type handlerType = _handler.GetType();
            Type commandType = _command.GetType();

            var handleMethod = handlerType.GetMethod("Handle", BindingFlags.Public | BindingFlags.Instance, null, new[] { commandType }, null);

            if (handleMethod == null)
            {
                throw new InvalidOperationException(
                    string.Format("Handler is of type {0} and has no public Handle({1}) method. "
                                  + "Since often there may be multiple classes representing a message with the same name, be sure to check the "
                                  + "handler handles the message in the assembly and namespace you are expecting.",
                                  handlerType,
                                  commandType));
            }

            handleMethod.Invoke(_handler, new object[] { _command });
        }
    }
}