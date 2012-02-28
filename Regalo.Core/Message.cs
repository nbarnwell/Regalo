using System;
using System.Collections;
using System.Text;

namespace Regalo.Core
{
    public abstract class Message
    {
        public string AggregateId { get; set; }

        protected Message(string aggregateId)
        {
            if (aggregateId == null) throw new ArgumentNullException("aggregateId");

            AggregateId = aggregateId;
        }

        public override string ToString()
        {
            // Event or Command?
            string messageTypeString = GetMessageTypeAsString();

            var type = GetType();
            var properties = type.GetProperties();
            var propertyList = new StringBuilder();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == "AggregateId") continue;

                if (propertyList.Length > 0) propertyList.Append(", ");

                propertyList.AppendFormat("{0}: ", propertyInfo.Name);

                if (!propertyInfo.PropertyType.IsValueType && propertyInfo.GetValue(this, null) == null)
                {
                    propertyList.Append("<null>");
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyList.AppendFormat("\"{0}\"", propertyInfo.GetValue(this, null));
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    propertyList.AppendFormat("{0}", propertyInfo.PropertyType.Name);
                }
                else
                {
                    propertyList.Append(propertyInfo.GetValue(this, null).ToString());
                }
            }

            return string.Format("{0} {1} for aggregate {2} with {3}", type.Name, messageTypeString, AggregateId, propertyList);
        }

        protected abstract string GetMessageTypeAsString();

        public bool Equals(Message other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.AggregateId, AggregateId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Message)) return false;
            return Equals((Message)obj);
        }

        public override int GetHashCode()
        {
            return (AggregateId != null ? AggregateId.GetHashCode() : 0);
        }

        public static bool operator ==(Message left, Message right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Message left, Message right)
        {
            return !Equals(left, right);
        }
    }
}