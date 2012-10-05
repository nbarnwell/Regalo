using System;

namespace Regalo.Core
{
    public static class Resolver
    {
        private static Func<Type, object> _resolver;

        public static void SetResolver(Func<Type, object> resolver)
        {
            if (_resolver != null)
            {
                throw new InvalidOperationException("Resolver has already been set. Be sure to call ClearResolver() first if you deliberately wish to change the implementation.");
            }

            _resolver = resolver;
        }

        public static void ClearResolver()
        {
            _resolver = null;
        }

        internal static T Resolve<T>()
        {
            return (T)_resolver.Invoke(typeof(T));
        }
    }
}