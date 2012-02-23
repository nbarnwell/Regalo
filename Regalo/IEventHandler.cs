namespace Regalo
{
    public interface IEventHandler<TEvent>
    {
        void Handle(TEvent evt);
    }
}