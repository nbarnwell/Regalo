using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Regalo.ObjectCompare
{
    public class PropertyComparisonIgnoreList
    {
        private readonly IDictionary<RuntimeTypeHandle, HashSet<string>> _ignored = new Dictionary<RuntimeTypeHandle, HashSet<string>>();

        public void Add<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var me = expression.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException(
                    string.Format("The name of the property could not be established from the expression '{0}'", expression),
                    "expression");
            }

            var type = typeof(T);
            HashSet<string> propertiesIgnoredForType;
            if (false == _ignored.TryGetValue(type.TypeHandle, out propertiesIgnoredForType))
            {
                propertiesIgnoredForType = new HashSet<string>();
                _ignored.Add(type.TypeHandle, propertiesIgnoredForType);
            }

            if (false == propertiesIgnoredForType.Contains(me.Member.Name))
            {
                propertiesIgnoredForType.Add(me.Member.Name);
            }
        }

        public bool Contains(Type type, string propertyName)
        {
            HashSet<string> propertiesIgnoredForType;
            if (false == _ignored.TryGetValue(type.TypeHandle, out propertiesIgnoredForType))
            {
                return false;
            }

            return propertiesIgnoredForType.Contains(propertyName);
        }
    }
}