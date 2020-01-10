using DbUtils.Utils;
using System.Collections.Generic;
using System.Configuration;

namespace DbUtils
{
    public interface IDbUtils
    {
        IEnumerable<TableColumn> GetColumns(string table);

        List<string> GetTableNames();
    }

    public static class DbHelper
    {
        public static IDbUtils GetUtils(ConnectionStringSettings config)
        {
            return DbHelper.GetUtils(config.ProviderName, config.ConnectionString);
        }

        public static IDbUtils GetUtils(string ProviderName, string ConnectionString)
        {
            if (ProviderName == "Npgsql")
                return (IDbUtils)new NpgUtils(ConnectionString);
            if (ProviderName == "MySql.Data.MySqlClient")
                return (IDbUtils)new MySqlUtils(ConnectionString);
            if (ProviderName == "System.Data.SqlClient")
                return (IDbUtils)new MsSqlUtils(ConnectionString);
            if (ProviderName == "Oracle.ManagedDataAccess.Client")
                return (IDbUtils)new OracleUtils(ConnectionString);
            return (IDbUtils)new MsSqlUtils(ConnectionString);
        }
    }
}
