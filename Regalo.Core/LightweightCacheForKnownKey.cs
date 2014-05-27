using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public class LightweightCacheForKnownKey<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _valueProvider;
        private readonly IDictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();

        public LightweightCacheForKnownKey(Func<TKey, TValue> valueProvider)
        {
            if (valueProvider == null) throw new ArgumentNullException("valueProvider");
            _valueProvider = valueProvider;
        }

        public TValue GetValue(TKey key)
        {
            TValue value;
            if (!_cache.TryGetValue(key, out value))
            {
                value = _valueProvider(key);
                _cache.Add(key, value);
            }

            return value;
        }
    }
}