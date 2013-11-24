using System;
using Regalo.Core;

namespace Regalo.Testing
{
    public class ScenarioThenSetter<TEntity, THandler, TCommand> : IThenSetter<TEntity, THandler, TCommand> 
        where TEntity : AggregateRoot, new()
    {
        private readonly TEntity _entity;
        private readonly THandler _handler;
        private readonly TestingMessageHandlerContext<TEntity> _context;
        private readonly TCommand _command;

        public ScenarioThenSetter(TEntity entity, THandler handler, TestingMessageHandlerContext<TEntity> context, TCommand command)
        {
            if (context == null) throw new ArgumentNullException("context");

            _entity = entity;
            _handler = handler;
            _context = context;
            _command = command;
        }

        public IScenarioAssert<TEntity, THandler, TCommand> Then(Func<TEntity, TCommand, object[]> func)
        {
            var expected = func.Invoke(_entity, _command);
            return new ScenarioAssert<TEntity, THandler, TCommand>(_entity, _handler, _command, _context, expected);
        }
    }
}