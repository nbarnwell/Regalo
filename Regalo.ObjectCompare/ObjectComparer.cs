using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using ObjectCompare;

namespace Regalo.ObjectCompare
{
    public class ObjectComparer : IObjectComparer
    {
        private readonly IObjectComparerProvider _objectComparerProvider;
        private readonly PropertyComparisonIgnoreList _ignores;

        public ObjectComparer(IObjectComparerProvider objectComparerProvider)
        {
            if (objectComparerProvider == null) throw new ArgumentNullException("objectComparerProvider");

            _objectComparerProvider = objectComparerProvider;
            _ignores = objectComparerProvider.GetIgnoreList();
        }

        public ObjectComparisonResult AreEqual(object object1, object object2)
        {
            if (object1 == null && object2 == null)
            {
                return ObjectComparisonResult.Success();
            }

            if ((object1 == null) != (object2 == null))
            {
                return ObjectComparisonResult.Fail("Nullity differs. Object1 is {0} while Object2 is {1}.", object1 ?? "null", object2 ?? "null");
            }

            var object1Type = object1.GetType();
            var object2Type = object2.GetType();

            if (object1Type != object2Type)
            {
                // Let the type check go if they're both at least enumerable
                if (false == (object1.GetType().IsEnumerable() && object2.GetType().IsEnumerable()))
                {
                    return ObjectComparisonResult.Fail("Objects are of different type. Object1 is {0} while Object2 is {1}.", object1Type, object2Type);
                }
            }

            if (object1Type.IsPrimitive())
            {
                return ArePrimitivesEqual(object1, object2);
            }

            if (object1Type.IsEnumerable())
            {
                return AreObjectsInEnumerablesEqual(object1, object2);
            }

            return AreComplexObjectsEqual(object1, object2);
        }

        private static ObjectComparisonResult ArePrimitivesEqual(object value2, object value1)
        {
            if (!value2.Equals(value1))
            {
                return ObjectComparisonResult.Fail("Primitive values differ. Value1: {0}, Value2: {1}.", value1, value2);
            }

            return ObjectComparisonResult.Success();
        }

        private ObjectComparisonResult AreComplexObjectsEqual(object object1, object object2)
        {
            var typeBeingCompared = object1.GetType();
            var properties = typeBeingCompared.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (false == properties.Any())
            {
                return ObjectComparisonResult.Success();
            }

            foreach (var property in properties)
            {
                if (_ignores.Contains(typeBeingCompared, property.Name))
                {
                    continue;
                }

                var value1 = property.GetValue(object1, null);
                var value2 = property.GetValue(object2, null);

                var comparer = _objectComparerProvider.ComparerFor(property.PropertyType);
                var result = comparer.AreEqual(value1, value2);

                if (!result.AreEqual)
                {
                    return result;
                }
            }

            return ObjectComparisonResult.Success();
        }

        private ObjectComparisonResult AreObjectsInEnumerablesEqual(object value1, object value2)
        {
            var enumerator1 = ((IEnumerable)value1).GetEnumerator();
            var enumerator2 = ((IEnumerable)value2).GetEnumerator();

            bool hasNext1 = enumerator1.MoveNext();
            bool hasNext2 = enumerator2.MoveNext();
            var enumeratorType = enumerator1.Current.GetType();

            while (hasNext1 && hasNext2)
            {
                var comparer = _objectComparerProvider.ComparerFor(enumeratorType);
                var result = comparer.AreEqual(enumerator1.Current, enumerator2.Current);
                if (!result.AreEqual)
                {
                    return result;
                }

                hasNext1 = enumerator1.MoveNext();
                hasNext2 = enumerator2.MoveNext();
            }

            if (hasNext1 != hasNext2)
            {
                return ObjectComparisonResult.Fail("Enumerable properties have different lengths.");
            }

            return ObjectComparisonResult.Success();
        }
    }
}