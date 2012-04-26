namespace Regalo.Core
{
    public abstract class Event : Message
    {
        protected override string GetMessageTypeAsString()
        {
            return "event";
        }
    }
}