namespace Regalo.Core
{
    public interface ICommandProcessor
    {
        void Process<TCommand>(TCommand command);
    }
}