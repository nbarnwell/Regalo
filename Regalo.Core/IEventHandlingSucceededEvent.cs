namespace Regalo.Core
{
    public interface IEventHandlingSucceededEvent<out TEvent>
    {
        TEvent Evt { get; }
    }
}