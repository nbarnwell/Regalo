using System;
using Regalo.Core;

namespace Regalo.RavenDB.Tests.Unit.DomainModel.Customers
{
    public class CustomerSignedUp : Event
    {
        public Guid AggregateId { get; set; }

        public CustomerSignedUp(Guid customerId)
        {
            AggregateId = customerId;
        }

        public bool Equals(CustomerSignedUp other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.AggregateId.Equals(AggregateId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CustomerSignedUp)) return false;
            return Equals((CustomerSignedUp)obj);
        }

        public override int GetHashCode()
        {
            return AggregateId.GetHashCode();
        }

        public static bool operator ==(CustomerSignedUp left, CustomerSignedUp right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomerSignedUp left, CustomerSignedUp right)
        {
            return !Equals(left, right);
        }
    }
}