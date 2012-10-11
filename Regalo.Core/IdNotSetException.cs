using System;
using System.Runtime.Serialization;

namespace Regalo.Core
{
    [Serializable]
    public class IdNotSetException : Exception
    {
        public IdNotSetException()
            : base(GetMessage())
        {
        }

        private static string GetMessage()
        {
            return "All initialised aggregate roots need to be assigned an ID. Ensure " +
                   "the first behaviour invoked on an aggregate emits an event " +
                   "representing the creation of that aggregate and storing the value" +
                   "of it's ID. Be sure to implement an Apply() method on the " +
                   "aggregate that will set the AggregateRoot.Id property";
        }
    }
}