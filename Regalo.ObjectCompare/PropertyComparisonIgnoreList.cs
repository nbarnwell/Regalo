using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Regalo.ObjectCompare
{
    public class PropertyComparisonIgnoreList
    {
        private readonly IList<Ignore> _ignored = new List<Ignore>(); 

        public void Add<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var me = expression.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException(
                    string.Format("The name of the property could not be established from the expression '{0}'", expression),
                    "expression");
            }

            var owningType = typeof(T);
            var propertyToIgnore = me.Member.Name;

            var existing = _ignored.SingleOrDefault(WhereIgnored(owningType, propertyToIgnore));

            if (null == existing)
            {
                _ignored.Add(new Ignore(owningType, propertyToIgnore));
            }
        }

        private static Func<Ignore, bool> WhereIgnored(Type owningType, string propertyToIgnore)
        {
            return x => owningType.IsAssignableFrom(x.Type)
                        && x.Name.Equals(propertyToIgnore, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Contains(Type owningType, string propertyName)
        {
            return _ignored.Any(WhereIgnored(owningType, propertyName));
        }

        private class Ignore
        {
            public Type   Type { get; private set; }
            public string Name { get; private set; }

            public Ignore(Type type, string name)
            {
                Type = type;
                Name = name;
            }
        }
    }
}