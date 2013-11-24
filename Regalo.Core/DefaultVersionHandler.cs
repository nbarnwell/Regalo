using System;

namespace Regalo.Core
{
    public class DefaultVersionHandler : IVersionHandler
    {
        public Guid GetVersion(object evt)
        {
            return ((Event)evt).Version;
        }

        public void SetParentVersion(object evt, Guid? parentVersion)
        {
            ((Event)evt).ParentVersion = parentVersion;
        }
    }
}