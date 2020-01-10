using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DbUtils.Utils
{
    public class NpgUtils : IDbUtils
    {
        protected string connstr { get; set; }

        public NpgUtils(string connstr)
        {
            this.connstr = connstr;
        }

        public IEnumerable<TableColumn> GetColumns(string table)
        {
            string sql = string.Format("select {0} from information_schema.tables t1 inner join information_schema.columns t2 on t1.TABLE_NAME = t2.TABLE_NAME where t1.table_schema ='public' and t1.table_name = @table", (object)string.Format("\r\n1 as COLUMN_ID, \r\nt2.COLUMN_NAME, \r\nt2.DATA_TYPE, \r\n1 as DATA_LENGTH, \r\nt2.is_nullable as NULLABLE,\r\n\r\n(SELECT pg_catalog.col_description(c.oid, t2.ordinal_position::int) FROM pg_catalog.pg_class c \r\nWHERE c.oid = (SELECT '{0}'::regclass::oid)\r\nAND c.relname = t2.table_name) as COMMENTS", (object)table));
            List<NpgUtils.TableColumnsItem> source;
            using (NpgsqlConnection cnn = new NpgsqlConnection(this.connstr))
                source = cnn.Query<NpgUtils.TableColumnsItem>(sql, (object)new
                {
                    table = table
                }, (IDbTransaction)null, true, new int?(), new CommandType?()).AsList<NpgUtils.TableColumnsItem>();
            return source.Select<NpgUtils.TableColumnsItem, TableColumn>((Func<NpgUtils.TableColumnsItem, TableColumn>)(t => new TableColumn()
            {
                id = t.COLUMN_ID,
                name = t.COLUMN_NAME,
                comments = t.COMMENTS,
                null_able = t.NULLABLE == "YES",
                type = t.DATA_TYPE
            }));
        }

        public List<string> GetTableNames()
        {
            using (NpgsqlConnection cnn = new NpgsqlConnection(this.connstr))
                return cnn.Query<string>("SELECT table_name FROM information_schema.tables where table_schema ='public' order by table_name", (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).AsList<string>();
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
