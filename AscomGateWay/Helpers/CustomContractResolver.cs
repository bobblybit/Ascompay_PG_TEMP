using Newtonsoft.Json.Serialization;

namespace AscomPayPG.Helpers
{
    public class CustomContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            // Keep 'sendername' in lowercase and others in camelCase
            if (propertyName.Equals("sendername", StringComparison.OrdinalIgnoreCase))
            {
                return propertyName.ToLower(); // Keep 'sendername' in lowercase
            }

            // Keep 'sendername' in lowercase and others in camelCase
            if (propertyName.Equals("senderaccountnumber", StringComparison.OrdinalIgnoreCase))
            {
                return propertyName.ToLower(); // Keep 'sendername' in lowercase
            }

            return base.ResolvePropertyName(propertyName); // Use camelCase for others
        }
    }
}
