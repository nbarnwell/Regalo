using System;
using Regalo.Core;

namespace Regalo.Testing
{
    public class ScenarioHandlerSetter<TEntity> : IHandlerSetter<TEntity> 
        where TEntity : AggregateRoot, new()
    {
        private readonly TestingMessageHandlerContext<TEntity> _context;

        public ScenarioHandlerSetter(TestingMessageHandlerContext<TEntity> context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
        }

        public IGivenSetter<TEntity, THandler> HandledBy<THandler>(THandler handler)
        {
            // TODO: Some sort of IoC style to build the handler with 
            // an event publisher and repository of some kind
            return new ScenarioGivenSetter<TEntity, THandler>(handler, _context);
        }
    }
}