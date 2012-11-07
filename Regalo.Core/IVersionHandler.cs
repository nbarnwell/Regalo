using System;

namespace Regalo.Core
{
    public interface IVersionHandler
    {
        Guid GetVersion(object evt);
        void SetParentVersion(object evt, Guid? parentVersion);
    }
}