namespace EventSorcerer
{
    public abstract class Event : Message
    {
        protected Event(string aggregateId) : base(aggregateId)
        {
        }

        protected override string GetMessageTypeAsString()
        {
            return "event";
        }
    }
}