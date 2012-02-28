namespace Regalo.Core
{
    public interface ICommandBus
    {
        void Send<TCommand>(TCommand command);
    }
}