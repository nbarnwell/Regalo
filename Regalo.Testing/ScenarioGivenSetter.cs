using System;
using Regalo.Core;

namespace Regalo.Testing
{
    public class ScenarioGivenSetter<TEntity, THandler> : IGivenSetter<TEntity, THandler> 
        where TEntity : AggregateRoot, new()
    {
        private readonly THandler _handler;
        private readonly TestingMessageHandlerContext<TEntity> _context;

        public ScenarioGivenSetter(THandler handler, TestingMessageHandlerContext<TEntity> context)
        {
            if (context == null) throw new ArgumentNullException("context");

            _handler = handler;
            _context = context;
        }

        public IWhenSetter<TEntity, THandler> Given(ITestDataBuilder<TEntity> testDataBuilder)
        {
            var entity = testDataBuilder.Build();
            return new ScenarioWhenSetter<TEntity, THandler>(entity, _handler, _context);
        }
    }
}