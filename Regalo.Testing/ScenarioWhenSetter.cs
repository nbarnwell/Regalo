using System;
using Regalo.Core;

namespace Regalo.Testing
{
    public class ScenarioWhenSetter<TEntity, THandler> : IWhenSetter<TEntity, THandler> 
        where TEntity : AggregateRoot, new()
    {
        private readonly TEntity _entity;
        private readonly THandler _handler;
        private readonly TestingMessageHandlerContext<TEntity> _context;

        public ScenarioWhenSetter(TEntity entity, THandler handler, TestingMessageHandlerContext<TEntity> context)
        {
            if (context == null) throw new ArgumentNullException("context");

            _entity = entity;
            _handler = handler;
            _context = context;
        }

        public IThenSetter<TEntity, THandler, TCommand> When<TCommand>(Func<TEntity, TCommand> func)
        {
            var command = func.Invoke(_entity);
            return new ScenarioThenSetter<TEntity, THandler, TCommand>(_entity, _handler, _context, command);
        }
    }
}