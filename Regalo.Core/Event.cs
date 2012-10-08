using System;

namespace Regalo.Core
{
    public abstract class Event : Message
    {
        protected Event()
        {
            Version = Guid.NewGuid();
        }

        public Guid Version { get; set; }

        public Guid ParentVersion { get; set; }

        protected override string GetMessageTypeAsString()
        {
            return "event";
        }
    }
}