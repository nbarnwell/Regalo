using NUnit.Framework;
using Regalo.Core;
using Regalo.Core.EventSourcing;

namespace Regalo.Testing
{
    public class ApplicationServiceTestBase<TEntity> 
        where TEntity : AggregateRoot, new()
    {
        protected TestingMessageHandlerContext<TEntity> Context { get; set; } 

        [SetUp]
        public void SetUp()
        {
            var eventStore = new InMemoryEventStore();
            var repository = new EventSourcingRepository<TEntity>(eventStore, new StrictConcurrencyMonitor());
            var eventBus = new FakeEventBus();
            Context = new TestingMessageHandlerContext<TEntity>(repository, eventBus);
        }

        [TearDown]
        public void TearDown()
        {
            Context = null;
        }
    }
}