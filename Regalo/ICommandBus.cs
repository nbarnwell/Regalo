namespace Regalo
{
    public interface ICommandBus
    {
        void Send<TCommand>(TCommand command);
    }
}