using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectCompare;

namespace Regalo.ObjectCompare
{
    public class ObjectComparer : IObjectComparer
    {
        private readonly IObjectComparerProvider _objectComparerProvider;
        private readonly IList<PropertyInfo> _properties;

        public ObjectComparer(Type type, IObjectComparerProvider objectComparerProvider)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (objectComparerProvider == null) throw new ArgumentNullException("objectComparerProvider");

            _properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _objectComparerProvider = objectComparerProvider;
        }

        public bool AreEqual(object object1, object object2)
        {
            if (object1 == object2)
            {
                return true;
            }

            if ((object1 == null) != (object2 == null))
            {
                return false;
            }

            if (!ArePropertiesEqual(object1, object2)) return false;

            return true;
        }

        private bool ArePropertiesEqual(object object1, object object2)
        {
            if (false == _properties.Any())
            {
                return true;
            }

            foreach (var property in _properties)
            {
                var value1 = property.GetValue(object1, null);
                var value2 = property.GetValue(object2, null);

                if (property.PropertyType.IsPrimitive())
                {
                    if (!ArePrimitivesEqual(value2, value1)) return false;
                }
                else if (property.PropertyType.IsEnumerable())
                {
                    if (!AreObjectsInEnumerablesEqual(value1, value2)) return false;
                }
                else
                {
                    if (!AreComplexObjectsEqual(property, value1, value2)) return false;
                }
            }
            return true;
        }

        private static bool ArePrimitivesEqual(object value2, object value1)
        {
            if (!value2.Equals(value1))
            {
                return false;
            }
            return true;
        }

        private bool AreComplexObjectsEqual(PropertyInfo property, object value1, object value2)
        {
            var comparer = _objectComparerProvider.ComparerFor(property.PropertyType);
            if (!comparer.AreEqual(value1, value2))
            {
                return false;
            }
            return true;
        }

        private bool AreObjectsInEnumerablesEqual(object value1, object value2)
        {
            var enumerator1 = ((IEnumerable)value1).GetEnumerator();
            var enumerator2 = ((IEnumerable)value2).GetEnumerator();

            bool hasNext1 = enumerator1.MoveNext();
            bool hasNext2 = enumerator2.MoveNext();
            while (hasNext1 && hasNext2)
            {
                var comparer = _objectComparerProvider.ComparerFor(enumerator1.Current.GetType());
                if (!comparer.AreEqual(enumerator1.Current, enumerator2.Current))
                {
                    return false;
                }

                hasNext1 = enumerator1.MoveNext();
                hasNext2 = enumerator2.MoveNext();
            }

            if (hasNext1 != hasNext2) return false;
            return true;
        }
    }
}