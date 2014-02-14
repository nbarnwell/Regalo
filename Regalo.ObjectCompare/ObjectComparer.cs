using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectCompare;

namespace Regalo.ObjectCompare
{
    public class ObjectComparer : IObjectComparer
    {
        private readonly Stack<string> _propertyComparisonStack = new Stack<string>();
        private readonly IList<object> _circularReferenceChecklist = new List<object>();
        private readonly PropertyComparisonIgnoreList _ignores = new PropertyComparisonIgnoreList();

        public ObjectComparisonResult AreEqual(object object1, object object2)
        {
            if (object1 == null && object2 == null)
            {
                return ObjectComparisonResult.Success();
            }

            if ((object1 == null) != (object2 == null))
            {
                return ObjectComparisonResult.Fail(_propertyComparisonStack, "Nullity differs. Object1 is {0} while Object2 is {1}.", object1 ?? "null", object2 ?? "null");
            }

            var object1Type = object1.GetType();
            var object2Type = object2.GetType();

            if (object1Type != object2Type)
            {
                // Let the type check go if they're both at least enumerable
                if (false == (object1.GetType().IsEnumerable() && object2.GetType().IsEnumerable()))
                {
                    return ObjectComparisonResult.Fail(_propertyComparisonStack, "Objects are of different type. Object1 is {0} while Object2 is {1}.", object1Type, object2Type);
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

        public IObjectComparer Ignore<T, TProperty>(Expression<Func<T, TProperty>> ignore)
        {
            _ignores.Add(ignore);
            return this;
        }

        private ObjectComparisonResult ArePrimitivesEqual(object value2, object value1)
        {
            if (!value2.Equals(value1))
            {
                return ObjectComparisonResult.Fail(_propertyComparisonStack, "Primitive values differ. Value1: {0}, Value2: {1}.", value1, value2);
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
                if (false == _ignores.Contains(typeBeingCompared, property.Name))
                {
                    _propertyComparisonStack.Push(property.Name);
                    try
                    {
                        var value1 = property.GetValue(object1, null);

                        if (_circularReferenceChecklist.Contains(value1))
                        {
                            return ObjectComparisonResult.Success();
                        }

                        _circularReferenceChecklist.Add(value1);

                        var value2 = property.GetValue(object2, null);

                        // NOTE: Recursion
                        var result = AreEqual(value1, value2);

                        if (!result.AreEqual)
                        {
                            return result;
                        }
                    }
                    finally
                    {
                        _propertyComparisonStack.Pop();
                    }
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

            while (hasNext1 && hasNext2)
            {
                var result = AreEqual(enumerator1.Current, enumerator2.Current);
                if (!result.AreEqual)
                {
                    return result;
                }

                hasNext1 = enumerator1.MoveNext();
                hasNext2 = enumerator2.MoveNext();
            }

            if (hasNext1 != hasNext2)
            {
                return ObjectComparisonResult.Fail(_propertyComparisonStack, "Enumerable properties have different lengths.");
            }

            return ObjectComparisonResult.Success();
        }
    }
}