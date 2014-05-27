using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public class LightweightCacheForRuntimeKey<TInput, TKey, TValue>
    {
        private readonly Func<TInput, TKey> _keyProvider;
        private readonly Func<TInput, TKey, TValue> _valueProvider;
        private readonly IDictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();

        public LightweightCacheForRuntimeKey(Func<TInput, TKey> keyProvider, Func<TInput, TKey, TValue> valueProvider) 
        {
            if (keyProvider == null) throw new ArgumentNullException("keyProvider");
            if (valueProvider == null) throw new ArgumentNullException("valueProvider");

            _keyProvider = keyProvider;
            _valueProvider = valueProvider;
        }

        public TValue GetValue(TInput input)
        {
            var key = _keyProvider(input);

            TValue value;
            if (!_cache.TryGetValue(key, out value))
            {
                value = _valueProvider(input, key);
                _cache.Add(key, value);
            }

            return value;
        }
    }
}