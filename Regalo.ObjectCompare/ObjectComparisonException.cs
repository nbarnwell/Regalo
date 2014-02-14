using System;
using System.Collections.Generic;

namespace Regalo.ObjectCompare
{
    public class ObjectComparisonException : Exception
    {
        public ObjectComparisonException(string message) : base(message)
        {
        }
    }
}