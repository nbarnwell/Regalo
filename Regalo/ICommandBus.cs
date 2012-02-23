namespace EventSorcerer
{
    public interface ICommandBus
    {
        void Send<TCommand>(TCommand command);
    }
}