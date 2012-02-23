namespace EventSorcerer
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}