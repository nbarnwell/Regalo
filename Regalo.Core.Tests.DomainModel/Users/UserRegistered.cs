using System;

namespace Regalo.Core.Tests.DomainModel.Users
{
    public class UserRegistered : Event
    {
        public Guid AggregateId { get; set; }

        public UserRegistered(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public bool Equals(UserRegistered other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.AggregateId.Equals(AggregateId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(UserRegistered)) return false;
            return Equals((UserRegistered)obj);
        }

        public override int GetHashCode()
        {
            return AggregateId.GetHashCode();
        }

        public static bool operator ==(UserRegistered left, UserRegistered right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserRegistered left, UserRegistered right)
        {
            return !Equals(left, right);
        }
    }
}