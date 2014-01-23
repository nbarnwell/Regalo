namespace Regalo.ObjectCompare
{
    public class ObjectComparisonResult
    {
        public static bool ThrowOnFail { get; set; }

        public bool   AreEqual         { get; private set; }
        public string InequalityReason { get; private set; }

        public ObjectComparisonResult(bool areEqual, string inequalityReason)
        {
            AreEqual = areEqual;
            InequalityReason = inequalityReason;
        }

        public static ObjectComparisonResult Success()
        {
            return new ObjectComparisonResult(true, "");
        }

        public static ObjectComparisonResult Fail(string reasonFormat, params object[] reasonArgs)
        {
            var inequalityReason = string.Format(reasonFormat, reasonArgs);

            if (ThrowOnFail)
            {
                throw new ObjectComparisonException(inequalityReason);
            }

            return new ObjectComparisonResult(false, inequalityReason);
        }
    }
}