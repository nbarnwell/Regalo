using System;
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
            var eventsStoredToEventStore = _context.GetGeneratedEvents();

            var comparerProvider = new ObjectComparerProvider();
            var comparer = comparerProvider.ComparerFor(typeof(object[]));
            if (false == comparer.AreEqual(_expected, eventsStoredToEventStore))
            {
                throw new AssertionException("Actual events did not match expected events.");
            }
        }
    }
}