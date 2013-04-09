namespace Regalo.Core
{
    public class CommandProcessor : MessageProcessorBase, ICommandProcessor
    {
        public CommandProcessor(ILogger logger) 
            : base(logger)
        {
        }

        public void Process<TCommand>(TCommand command)
        {
            HandleMessage(command, typeof(ICommandHandler<>));
        }
    }
}