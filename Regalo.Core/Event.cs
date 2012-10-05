using System;

namespace Regalo.Core
{
    public abstract class Event : Message
    {
        public Guid Version { get; set; }
        public Guid ParentVersion { get; set; }

        protected override string GetMessageTypeAsString()
        {
            return "event";
        }
    }
}