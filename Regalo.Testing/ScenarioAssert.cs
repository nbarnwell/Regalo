using System;
using NUnit.Framework;
using Regalo.Core;
using Regalo.ObjectCompare;

namespace Regalo.Testing
{
    public class ScenarioAssert<TEntity, THandler, TCommand> : ScenarioAssertBase<THandler, TCommand>, IScenarioAssert<TEntity, THandler, TCommand> 
        where TEntity : AggregateRoot, new()
    {
        private readonly TEntity _entity;
        private readonly TestingMessageHandlerContext<TEntity> _context;
        private readonly object[] _expected;

        public ScenarioAssert(TEntity entity, THandler handler, TCommand command, TestingMessageHandlerContext<TEntity> context, object[] expected) 
            : base(handler, command)
        {
            if (context == null) throw new ArgumentNullException("context");

            _entity = entity;
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
    }
}