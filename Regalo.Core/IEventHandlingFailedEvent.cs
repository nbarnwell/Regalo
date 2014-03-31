using System;

namespace Regalo.Core
{
    public interface IEventHandlingFailedEvent<out TEvent>
    {
        TEvent Evt { get; }
        Exception Exception { get; }
    }
}