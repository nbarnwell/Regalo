using System.Collections;

namespace Regalo.Core.EventSourcing
{
    public class ConcurrencyConflict
    {
        public IEnumerable BaseEvents { get; private set; }
        public IEnumerable UnseenEvents { get; private set; }
        public IEnumerable UncommittedEvents { get; private set; }
        public string Message { get; private set; }

        public ConcurrencyConflict(string message, IEnumerable baseEvents, IEnumerable unseenEvents, IEnumerable uncommittedEvents)
        {
            BaseEvents = baseEvents;
            UnseenEvents = unseenEvents;
            UncommittedEvents = uncommittedEvents;
            Message = message;
        }
    }
}