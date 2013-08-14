using System;

namespace Regalo.Core
{
    public abstract class EventHandlingResultEvent
    {
        public Guid CorrelationId { get; set; }
    }
}