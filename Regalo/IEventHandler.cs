namespace EventSorcerer
{
    public interface IEventHandler<TEvent>
    {
        void Handle(TEvent evt);
    }
}