using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Regalo.ObjectCompare
{
    public class ObjectComparerProvider : IObjectComparerProvider
    {
        private readonly IDictionary<RuntimeTypeHandle, IObjectComparer> _cache = new Dictionary<RuntimeTypeHandle, IObjectComparer>();
        private readonly PropertyComparisonIgnoreList _ignores = new PropertyComparisonIgnoreList();

        public IObjectComparer ComparerFor(Type type)
        {
            IObjectComparer cached;
            var key = type.TypeHandle;
            if (!_cache.TryGetValue(key, out cached))
            {
                cached = new ObjectComparer(this);
                _cache.Add(key, cached);
            }

            return cached;
        }

        public IObjectComparerProvider Ignore<T, TProperty>(Expression<Func<T, TProperty>> ignore)
        {
            _ignores.Add(ignore);
            return this;
        }

        public PropertyComparisonIgnoreList GetIgnoreList()
        {
            return _ignores;
        }
    }
}