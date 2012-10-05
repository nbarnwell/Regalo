using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public interface IVersionHandler
    {
        void Append(IList<object> events, object evt);
        Guid GetCurrentVersion(IEnumerable<object> events);
        Guid GetVersion(object evt);
    }
}