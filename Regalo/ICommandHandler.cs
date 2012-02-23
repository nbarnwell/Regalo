namespace Regalo
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}