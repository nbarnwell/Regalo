using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public class LightweightCacheForRuntimeKey<TInput, TKey, TValue> : LightweightCache<TKey, TValue>
    {
        private readonly Func<TInput, TKey> _keyLoader;

        public LightweightCacheForRuntimeKey(Func<TInput, TKey> keyLoader, Func<TKey, TValue> valueLoader) 
            : base(valueLoader)
        {
            if (keyLoader == null) throw new ArgumentNullException("keyLoader");
            _keyLoader = keyLoader;
        }

        public TValue GetValue(TInput input)
        {
            var key = _keyLoader(input);
            return GetValue(key);
        }
    }

    public class LightweightCache<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _loader;
        private readonly IDictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();

        public LightweightCache(Func<TKey, TValue> loader)
        {
            if (loader == null) throw new ArgumentNullException("loader");
            _loader = loader;
        }

        public TValue GetValue(TKey key)
        {
            TValue value;
            if (!_cache.TryGetValue(key, out value))
            {
                value = _loader(key);
                _cache.Add(key, value);
            }

            return value;
        }
    }
}