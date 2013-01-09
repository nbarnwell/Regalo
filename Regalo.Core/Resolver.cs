using System;
using System.Collections.Generic;

namespace Regalo.Core
{
    public static class Resolver
    {
        private static Func<Type, object> _singleResolver;
        private static Func<Type, IEnumerable<object>> _multipleResolver;

        public static void SetResolvers(Func<Type, object> singleResolver, Func<Type, IEnumerable<object>> multipleResolver)
        {
            if (singleResolver == null) throw new ArgumentNullException("singleResolver");
            if (multipleResolver == null) throw new ArgumentNullException("multipleResolver");

            if (_singleResolver != null || _multipleResolver != null)
            {
                throw new InvalidOperationException("Resolvers have already been set. Be sure to call ClearResolvers() first if you deliberately wish to change the implementation.");
            }

            _singleResolver = singleResolver;
            _multipleResolver = multipleResolver;
        }

        public static void ClearResolvers()
        {
            _singleResolver = null;
            _multipleResolver = null;
        }

        internal static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        internal static object Resolve(Type type)
        {
            if (_singleResolver == null)
            {
                throw new InvalidOperationException("Resolvers have not been set. Be sure to call Regalo.Core.Resolver.SetResolvers() in your application initialisation.");
            }

            return _singleResolver.Invoke(type);
        }

        internal static IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)ResolveAll(typeof(T));
        }

        internal static IEnumerable<object> ResolveAll(Type type)
        {
            if (_multipleResolver == null)
            {
                throw new InvalidOperationException("Resolvers have not been set. Be sure to call Regalo.Core.Resolver.SetResolvers() in your application initialisation.");
            }

            return _multipleResolver.Invoke(type);
        }
    }
}