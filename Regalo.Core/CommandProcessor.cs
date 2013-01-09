namespace Regalo.Core
{
    public class CommandProcessor : MessageProcessorBase, ICommandProcessor
    {
        public void Process<TCommand>(TCommand command)
        {
            HandleMessage(command, typeof(ICommandHandler<>));
        }
    }
}