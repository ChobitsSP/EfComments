using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DbUtils.Utils
{
    public class MsSqlUtils : IDbUtils
    {
        protected string connstr { get; set; }

        public MsSqlUtils(string connstr)
        {
            this.connstr = connstr;
        }

        public IEnumerable<TableColumn> GetColumns(string table)
        {
            using (SqlConnection cnn = new SqlConnection(this.connstr))
                return (IEnumerable<TableColumn>)cnn.Query<TableColumn>("\r\nSELECT\r\n        id=a.colorder,\r\n        name=a.name,\r\n        type=b.name,\r\n        null_able = a.isnullable,\r\n        comments=isnull(g.[value],'')\r\n        FROM   syscolumns   a\r\n        left   join   systypes   b   on   a.xusertype=b.xusertype\r\n        inner   join   sysobjects   d   on   a.id=d.id     and   d.xtype='U'   and     d.name<>'dtproperties'\r\n        left   join   syscomments   e   on   a.cdefault=e.id\r\n        left   join   sys.extended_properties   g   on   a.id=g.major_id   and   a.colid=g.minor_id\r\n        left   join   sys.extended_properties   f   on   d.id=f.major_id   and   f.minor_id=0\r\n        where   d.name= @table\r\n        order   by   a.id,a.colorder\r\n", (object)new
                {
                    table = table
                }, (IDbTransaction)null, true, new int?(), new CommandType?()).AsList<TableColumn>();
        }

        public List<string> GetTableNames()
        {
            using (SqlConnection cnn = new SqlConnection(this.connstr))
                return cnn.Query<string>("select name from sysobjects where xtype='u' order by name", (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).AsList<string>();
        }

        public class RootObject
        {
            public string COLUMN_NAME { get; set; }

            public int ORDINAL_POSITION { get; set; }

            public string COLUMN_DEFAULT { get; set; }

            public string IS_NULLABLE { get; set; }

            public string DATA_TYPE { get; set; }

            public object CHARACTER_MAXIMUM_LENGTH { get; set; }

            public object CHARACTER_OCTET_LENGTH { get; set; }

            public object NUMERIC_PRECISION { get; set; }

            public object NUMERIC_PRECISION_RADIX { get; set; }

            public object NUMERIC_SCALE { get; set; }

            public object DATETIME_PRECISION { get; set; }

            public string CHARACTER_SET_CATALOG { get; set; }

            public string CHARACTER_SET_SCHEMA { get; set; }

            public string CHARACTER_SET_NAME { get; set; }

            public string COLLATION_CATALOG { get; set; }

            public string COLLATION_SCHEMA { get; set; }

            public string COLLATION_NAME { get; set; }

            public string DOMAIN_CATALOG { get; set; }

            public string DOMAIN_SCHEMA { get; set; }

            public string DOMAIN_NAME { get; set; }
        }
    }
}
