using System;
using System.Linq.Expressions;

namespace Regalo.ObjectCompare
{
    public interface IObjectComparerProvider
    {
        IObjectComparer ComparerFor(Type type);
        PropertyComparisonIgnoreList GetIgnoreList();
        IObjectComparerProvider Ignore<T, TProperty>(Expression<Func<T, TProperty>> ignore);
    }
}