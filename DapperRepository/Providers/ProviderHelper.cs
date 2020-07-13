using System;

namespace DapperRepository.Providers
{
    public class ProviderHelper
    {
        public static IProvider GetProvider(string providerName)
        {
            if (string.Equals(providerName, "SqlServer", StringComparison.InvariantCultureIgnoreCase))
                return new SqlServerProvider();

            return null;
        }
    }
}