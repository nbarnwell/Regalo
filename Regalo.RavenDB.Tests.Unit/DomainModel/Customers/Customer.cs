using System;
using System.Collections.Generic;
using Regalo.Core;

namespace Regalo.RavenDB.Tests.Unit.DomainModel.Customers
{
    public class Customer : AggregateRoot
    {
        private readonly ISet<string> _subscribedNewsletters = new HashSet<string>(); 

        public void Signup()
        {
            Record(new CustomerSignedUp(Guid.NewGuid()));
        }

        public void SubscribeToNewsletter(string newsletterName)
        {
            if (!string.IsNullOrWhiteSpace(newsletterName)) throw new InvalidOperationException("Newsletter name is not suitable for subscription.");
            if (_subscribedNewsletters.Contains(newsletterName)) throw new InvalidOperationException("Customer is already subscribed to this newsletter.");

            Record(new SubscribedToNewsletter(newsletterName));
        }

        public void AssignAccountManager(Guid accountManagerId, DateTime startDate)
        {
            if (startDate > DateTime.Today) throw new InvariantNotSatisfiedException("Cannot assign an account manager whose start date is in the future.");

            Record(new AssignedAccountManager(accountManagerId));
        }

        private void Apply(CustomerSignedUp evt)
        {
            Id = evt.AggregateId;
        }

        private void Apply(SubscribedToNewsletter evt)
        {
            _subscribedNewsletters.Add(evt.NewsletterName);
        }
    }

    public class AssignedAccountManager
    {
        public Guid AccountManagerId { get; private set; }

        public AssignedAccountManager(Guid accountManagerId)
        {
            AccountManagerId = accountManagerId;
        }

        public bool Equals(AssignedAccountManager other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.AccountManagerId.Equals(AccountManagerId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(AssignedAccountManager)) return false;
            return Equals((AssignedAccountManager)obj);
        }

        public override int GetHashCode()
        {
            return AccountManagerId.GetHashCode();
        }
    }
}