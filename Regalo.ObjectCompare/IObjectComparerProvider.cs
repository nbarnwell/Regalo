using System;

namespace Regalo.ObjectCompare
{
    public interface IObjectComparerProvider
    {
        IObjectComparer ComparerFor(Type type);
    }
}