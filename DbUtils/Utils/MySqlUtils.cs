using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DbUtils.Utils
{
    public class MySqlUtils : IDbUtils
    {
        protected string connstr { get; set; }

        public MySqlUtils(string connstr)
        {
            this.connstr = connstr;
        }

        public IEnumerable<TableColumn> GetColumns(string table)
        {
            throw new NotImplementedException();
        }

        public List<string> GetTableNames()
        {
            using (MySqlConnection cnn = new MySqlConnection(this.connstr))
                return cnn.Query<string>("select table_name from information_schema.tables \r\n            where table_schema='csdb' \r\n            and table_type='base table';", (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).AsList<string>();
        }
    }
}
