using DapperRepository.Providers.SqlServer;
using DapperRepository.Providers.MySql;
using System;

namespace DapperRepository.Providers
{
    public class ProviderHelper
    {
        public static IProvider GetProvider(string providerName)
        {
            if (string.Equals(providerName, "SqlServer", StringComparison.InvariantCultureIgnoreCase))
                return new SqlServerProvider();
            else if (string.Equals(providerName, "MySql", StringComparison.InvariantCultureIgnoreCase))
                return new MySqlProvider();

            return null;
        }
    }
}