using System.Collections;

namespace Regalo.Core.EventSourcing
{
    public class ConcurrencyConflict
    {
        public ConcurrencyConflict(string message, IEnumerable unseenEvents, IEnumerable uncommittedEvents)
        {
            UnseenEvents = unseenEvents;
            UncommittedEvents = uncommittedEvents;
            Message = message;
        }

        public IEnumerable UnseenEvents { get; private set; }

        public IEnumerable UncommittedEvents { get; private set; }

        public string Message { get; private set; }
    }
}