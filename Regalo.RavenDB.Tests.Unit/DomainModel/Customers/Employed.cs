using System;

namespace Regalo.RavenDB.Tests.Unit.DomainModel.Customers
{
    public class Employed
    {
        public Guid AggregateId { get; private set; }
        public DateTime StartDate { get; private set; }

        public Employed(Guid aggregateId, DateTime startDate)
        {
            AggregateId = aggregateId;
            StartDate = startDate;
        }

        public bool Equals(Employed other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.AggregateId.Equals(AggregateId) && other.StartDate.Equals(StartDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Employed)) return false;
            return Equals((Employed)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AggregateId.GetHashCode()*397) ^ StartDate.GetHashCode();
            }
        }
    }
}