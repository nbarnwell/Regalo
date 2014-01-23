using System;

namespace Regalo.ObjectCompare
{
    public class ObjectComparisonException : Exception
    {
        public ObjectComparisonException(string inequalityReason)
            : base(inequalityReason)
        {
        }
    }
}