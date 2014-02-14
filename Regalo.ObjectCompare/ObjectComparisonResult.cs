using System.Collections.Generic;
using System.Linq;

namespace Regalo.ObjectCompare
{
    public class ObjectComparisonResult
    {
        public static bool ThrowOnFail { get; set; }

        public bool                AreEqual                 { get; private set; }
        public string              InequalityReason         { get; private set; }

        private ObjectComparisonResult(bool areEqual, string inequalityReason)
        {
            AreEqual                 = areEqual;
            InequalityReason         = inequalityReason;
        }

        public static ObjectComparisonResult Success()
        {
            return new ObjectComparisonResult(true, "");
        }

        public static ObjectComparisonResult Fail(IEnumerable<string> propertyChainDescription, string reasonFormat, params object[] reasonArgs)
        {
            var inequalityReason = string.Format(reasonFormat, reasonArgs);
            var message = string.Format("{0}\r\n   at {1}", inequalityReason, FormatPropertyChainDescription(propertyChainDescription));

            if (ThrowOnFail)
            {
                throw new ObjectComparisonException(message);
            }

            return new ObjectComparisonResult(false, message);
        }

        private static string FormatPropertyChainDescription(IEnumerable<string> propertyChainDescription)
        {
            return string.Join(".", propertyChainDescription.Reverse());
        }
    }
}