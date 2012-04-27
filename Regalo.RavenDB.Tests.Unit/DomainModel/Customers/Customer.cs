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

        private void Apply(CustomerSignedUp evt)
        {
            Id = evt.AggregateId;
        }

        private void Apply(SubscribedToNewsletter evt)
        {
            _subscribedNewsletters.Add(evt.NewsletterName);
        }
    }
}