namespace Regalo.Core
{
    public class Conventions
    {
        private static string _aggregateIdPropertyName = "AggregateId";
        public static string AggregateIdPropertyName { get { return _aggregateIdPropertyName; } }
        
        public static void SetAggregateIdPropertyName(string value)
        {
            _aggregateIdPropertyName = value;
        }

        private static bool _aggregatesMustImplementApplyMethods = false;
        public static bool AggregatesMustImplementApplyMethods { get { return _aggregatesMustImplementApplyMethods; } }

        public static void SetAggregatesMustImplementApplymethods(bool value)
        {
            _aggregatesMustImplementApplyMethods = value;
        }
    }
}