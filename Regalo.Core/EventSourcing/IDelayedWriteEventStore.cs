namespace Regalo.Core.EventSourcing
{
    public interface IDelayedWriteEventStore : IEventStore
    {
        void Flush();
    }
}