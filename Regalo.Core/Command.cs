namespace Regalo.Core
{
    public class Command : Message
    {
        protected override string GetMessageTypeAsString()
        {
            return "command";
        }
    }
}