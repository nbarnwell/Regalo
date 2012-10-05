using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public interface IVersionHandler
    {
        void Append(IList<object> events, object evt);
        Guid FindCurrentVersion(IEnumerable<object> events);
        Guid GetVersion(object evt);
    }

    public class VersionHandler : IVersionHandler
    {
        public void Append(IList<object> events, object evt)
        {
            throw new NotImplementedException();
        }

        public Guid FindCurrentVersion(IEnumerable<object> events)
        {
            throw new NotImplementedException();
        }

        public Guid GetVersion(object evt)
        {
            throw new NotImplementedException();
        }
    }
}