using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DbUtils.Utils
{
    public class OracleUtils : IDbUtils
    {
        protected string connstr { get; set; }

        public OracleUtils(string connstr)
        {
            this.connstr = connstr;
        }

        public IEnumerable<TableColumn> GetColumns(string table)
        {
            string sql = string.Format("select {0} from user_col_comments t1 inner join user_tab_columns t2 on t1.COLUMN_NAME = t2.COLUMN_NAME and t1.TABLE_NAME = t2.TABLE_NAME where t1.table_name = '{1}' order by column_id asc", (object)"t2.COLUMN_ID,t1.COLUMN_NAME,COMMENTS,t2.DATA_TYPE,t2.DATA_LENGTH,t2.NULLABLE", (object)table);
            List<OracleUtils.TableColumnsItem> source;
            using (OracleConnection cnn = new OracleConnection(this.connstr))
                source = cnn.Query<OracleUtils.TableColumnsItem>(sql, (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).AsList<OracleUtils.TableColumnsItem>();
            return source.Select<OracleUtils.TableColumnsItem, TableColumn>((Func<OracleUtils.TableColumnsItem, TableColumn>)(t => new TableColumn()
            {
                id = t.COLUMN_ID,
                name = t.COLUMN_NAME,
                comments = t.COMMENTS,
                null_able = t.NULLABLE == "Y",
                type = t.DATA_TYPE
            }));
        }

        public List<string> GetTableNames()
        {
            using (OracleConnection cnn = new OracleConnection(this.connstr))
                return cnn.Query<string>("SELECT table_name FROM user_tables order by table_name", (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).AsList<string>();
        }

        public class TableColumnsItem
        {
            public int COLUMN_ID { get; set; }

            public string COLUMN_NAME { get; set; }

            public string COMMENTS { get; set; }

            public string DATA_TYPE { get; set; }

            public string NULLABLE { get; set; }
        }
    }
}
