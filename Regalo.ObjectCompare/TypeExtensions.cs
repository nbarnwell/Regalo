using System;
using System.Collections;
using System.Collections.Generic;

namespace Regalo.ObjectCompare
{
    public static class TypeExtensions
    {
        private static readonly ISet<RuntimeTypeHandle> __primitives = new HashSet<RuntimeTypeHandle>();

        static TypeExtensions()
        {
            __primitives.Add(typeof(bool).TypeHandle);
            __primitives.Add(typeof(byte).TypeHandle);
            __primitives.Add(typeof(sbyte).TypeHandle);
            __primitives.Add(typeof(char).TypeHandle);
            __primitives.Add(typeof(decimal).TypeHandle);
            __primitives.Add(typeof(double).TypeHandle);
            __primitives.Add(typeof(float).TypeHandle);
            __primitives.Add(typeof(int).TypeHandle);
            __primitives.Add(typeof(uint).TypeHandle);
            __primitives.Add(typeof(long).TypeHandle);
            __primitives.Add(typeof(ulong).TypeHandle);
            __primitives.Add(typeof(object).TypeHandle);
            __primitives.Add(typeof(short).TypeHandle);
            __primitives.Add(typeof(ushort).TypeHandle);
            __primitives.Add(typeof(string).TypeHandle); 
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsValueType || __primitives.Contains(type.TypeHandle);
        }

        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string)) return false;

            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}