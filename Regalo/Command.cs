namespace EventSorcerer
{
    public class Command : Message
    {
        public Command(string aggregateId) : base(aggregateId)
        {
        }

        protected override string GetMessageTypeAsString()
        {
            return "command";
        }
    }
}