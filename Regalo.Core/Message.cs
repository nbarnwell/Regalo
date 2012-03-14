using System;
using System.Collections;
using System.Text;

namespace Regalo.Core
{
    public abstract class Message
    {
        public override string ToString()
        {
            // Event or Command?
            string messageTypeString = GetMessageTypeAsString();

            var type = GetType();
            var properties = type.GetProperties();
            var propertyList = new StringBuilder();

            string aggregateId = "<not set>";

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == "AggregateId")
                {
                    aggregateId = propertyInfo.GetValue(this, null).ToString();
                    continue;
                }

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

            return string.Format("{0} {1} for aggregate {2} with {3}", type.Name, messageTypeString, aggregateId, propertyList);
        }

        protected abstract string GetMessageTypeAsString();
    }
}