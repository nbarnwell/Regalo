using System;
using System.Collections.Generic;

namespace Regalo.ObjectCompare
{
    public class ObjectComparisonException : Exception
    {
        public IEnumerable<string> PropertyChainDescription { get; private set; }

        public ObjectComparisonException(string inequalityReason, IEnumerable<string> propertyChainDescription)
            : base(inequalityReason)
        {
            PropertyChainDescription = propertyChainDescription;
        }
    }
}