using System.Collections.Generic;

namespace Regalo.ObjectCompare
{
    public class ObjectComparisonResult
    {
        public static bool ThrowOnFail { get; set; }

        public bool                AreEqual                 { get; private set; }
        public string              InequalityReason         { get; private set; }
        public IEnumerable<string> PropertyChainDescription { get; private set; }

        private ObjectComparisonResult(bool areEqual, string inequalityReason, IEnumerable<string> propertyChainDescription)
        {
            AreEqual                 = areEqual;
            InequalityReason         = inequalityReason;
            PropertyChainDescription = propertyChainDescription;
        }

        public static ObjectComparisonResult Success()
        {
            return new ObjectComparisonResult(true, "", null);
        }

        public static ObjectComparisonResult Fail(IEnumerable<string> propertyChainDescription, string reasonFormat, params object[] reasonArgs)
        {
            var inequalityReason = string.Format(reasonFormat, reasonArgs);

            if (ThrowOnFail)
            {
                throw new ObjectComparisonException(inequalityReason, propertyChainDescription);
            }

            return new ObjectComparisonResult(false, inequalityReason, propertyChainDescription);
        }
    }
}