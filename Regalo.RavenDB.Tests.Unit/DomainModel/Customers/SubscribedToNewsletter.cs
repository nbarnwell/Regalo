using Regalo.Core;

namespace Regalo.RavenDB.Tests.Unit.DomainModel.Customers
{
    public class SubscribedToNewsletter : Event
    {
        public string NewsletterName { get; private set; }

        public SubscribedToNewsletter(string newsletterName)
        {
            NewsletterName = newsletterName;
        }

        public bool Equals(SubscribedToNewsletter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.NewsletterName, NewsletterName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(SubscribedToNewsletter)) return false;
            return Equals((SubscribedToNewsletter)obj);
        }

        public override int GetHashCode()
        {
            return (NewsletterName != null ? NewsletterName.GetHashCode() : 0);
        }

        public static bool operator ==(SubscribedToNewsletter left, SubscribedToNewsletter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SubscribedToNewsletter left, SubscribedToNewsletter right)
        {
            return !Equals(left, right);
        }
    }
}