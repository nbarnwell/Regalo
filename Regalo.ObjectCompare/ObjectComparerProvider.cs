using System;
using System.Collections.Generic;

namespace Regalo.ObjectCompare
{
    public class ObjectComparerProvider : IObjectComparerProvider
    {
        private readonly IDictionary<RuntimeTypeHandle, IObjectComparer> _cache =
            new Dictionary<RuntimeTypeHandle, IObjectComparer>();

        public IObjectComparer ComparerFor(Type type)
        {
            IObjectComparer cached;
            var key = type.TypeHandle;
            if (!_cache.TryGetValue(key, out cached))
            {
                cached = new ObjectComparer(type, this);
                _cache.Add(key, cached);
            }

            return cached;
        }
    }
}