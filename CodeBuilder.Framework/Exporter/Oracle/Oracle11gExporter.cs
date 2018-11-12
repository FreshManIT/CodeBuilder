using System;
using System.Text;
namespace CodeBuilder.DataSource.Exporter
{
    using FreshCommonUtility.Dapper;
    using FreshCommonUtility.SqlHelper;
    using PhysicalDataModel;
    using System.Data;
    using Dapper;
    using FreshCommonUtility.DataConvert;
    using System.Linq;
    using System.Collections.Generic;

    public class Oracle11gExporter : BaseExporter
    {
        #region IExporter Members

        public override Model Export(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            Model model = new Model { Database = "Oracle11g" };
            model.Tables = GetTables(connectionString);
            model.Views = GetViews(connectionString);
            return model;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 获取表格列表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Tables GetTables(string connectionString)
        {
            var allColumns = GetColumns(null, connectionString);
            if (allColumns == null || allColumns.Count < 1)
            {
                return null;
            }
            var allPrimaryKeys = GetPrimaryKeys(null, connectionString);

            Tables tables = new Tables();
            string sqlCmd = "SELECT table_name,cluster_name FROM user_tables";
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.Oracle);
            using (IDbConnection conn = SqlConnectionHelper.GetOpenConnection(connectionString))
            {
                var dr = conn.ExecuteReader(sqlCmd);
                while (dr.Read())
                {
                    string id = dr.GetString(0);
                    string displayName = dr.GetString(0);
                    string name = dr.GetString(0);
                    string comment = dr.IsDBNull(1) ? string.Empty : dr.GetString(1);
                    var itemColumns = new Columns();
                    allColumns.Where(item => item.TableName == id).ToList().ForEach(f =>
                    {
                        itemColumns.Add(f.Id, f);
                    });
                    var itemPrimaryKeys = new Columns();
                    allPrimaryKeys.Where(item => item.TableName == id).ToList().ForEach(r =>
                    {
                        itemPrimaryKeys.Add(r.Id, r);
                    });
                    Table table = new Table(id, displayName, name, comment)
                    {
                        OriginalName = name,
                        Columns = itemColumns,
                        PrimaryKeys = itemPrimaryKeys
                    };
                    tables.Add(id, table);
                }
                dr.Close();

                return tables;
            }

        }

        /// <summary>
        /// 获取视图列表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Views GetViews(string connectionString)
        {
            var allColumns = GetColumns(null, connectionString);
            if (allColumns == null || allColumns.Count < 1)
            {
                return null;
            }
            Views views = new Views(10);
            string sqlCmd = $"select view_name,editioning_view from user_views";
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.Oracle);
            using (IDbConnection conn = SqlConnectionHelper.GetOpenConnection(connectionString))
            {
                var dr = conn.ExecuteReader(sqlCmd);
                while (dr.Read())
                {
                    string id = dr.GetString(0);
                    string displayName = dr.GetString(0);
                    string name = dr.GetString(0);
                    string comment = string.Empty;
                    var itemColumns = new Columns();
                    allColumns.Where(item => item.TableName == id).ToList().ForEach(f =>
                    {
                        itemColumns.Add(f.Id, f);
                    });
                    View view = new View(id, displayName, name, comment)
                    {
                        OriginalName = name,
                        Columns = itemColumns
                    };
                    views.Add(id, view);
                }
                dr.Close();
                return views;
            }

        }

        /// <summary>
        /// 获取表中的字段
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private List<Column> GetColumns(string tablename, string connectionString)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(@"SELECT
    utc.table_name,
    utc.COLUMN_ID,
    utc.column_name,
    utc.data_type,
    utc.data_length,
    utc.DATA_PRECISION,
    utc.NULLABLE,
    utc.data_length,
    utc.data_default,
    UCC.comments
FROM
    user_tab_columns utc
LEFT JOIN user_col_comments ucc ON utc.table_name = ucc.table_name
AND utc.column_name = ucc.column_name ");
            if (!string.IsNullOrEmpty(tablename))
            {
                sqlBuilder.AppendFormat(@" WHERE UTC.table_name = '{0}' ", tablename);
            }
            sqlBuilder.Append(@"
ORDER BY
    table_name,
    COLUMN_ID ");
            List<Column> columns = new List<Column>();
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.Oracle);
            using (IDbConnection conn = SqlConnectionHelper.GetOpenConnection(connectionString))
            {
                var dr = conn.ExecuteReader(sqlBuilder.ToString());
                while (dr.Read())
                {
                    string id = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                    string displayName = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                    string name = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                    string dataType = dr.IsDBNull(3) ? string.Empty : dr.GetString(3);
                    string defaultValue = dr.IsDBNull(8) ? string.Empty : dr.GetValue(8).ToString();
                    string isNullable = dr.IsDBNull(6) ? string.Empty : dr.GetString(6);
                    string length = dr.IsDBNull(7) ? string.Empty : dr.GetValue(7).ToString();
                    string identity = string.Empty;
                    string comment = dr.IsDBNull(9) ? string.Empty : dr.GetValue(9).ToString();

                    Column column = new Column(id, displayName, name, dataType, comment);
                    column.Length = DataTypeConvertHelper.ToInt(length);
                    column.IsAutoIncremented = false;
                    column.IsNullable = isNullable.Equals("Y");
                    column.DefaultValue = defaultValue;
                    column.DataType = dataType;
                    column.OriginalName = name;
                    column.TableName = dr.IsDBNull(0) ? string.Empty : dr.GetString(0);
                    columns.Add(column);
                }
                dr.Close();

                return columns;
            }
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private List<Column> GetPrimaryKeys(string tableName, string connectionString)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(@"select  col.* from user_constraints con, user_cons_columns col where con.constraint_name = col.constraint_name and con.constraint_type = 'P' ");
            if (!string.IsNullOrEmpty(tableName))
            {
                sqlBuilder.AppendFormat(" and col.table_name = '{0}' ", tableName);
            }
            return GetKeys(connectionString, sqlBuilder.ToString(), tableName);
        }

        /// <summary>
        /// GetKeys
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        private List<Column> GetKeys(string connectionString, string sqlCmd, string tableName)
        {
            List<Column> columns = new List<Column>();
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.Oracle);
            using (IDbConnection conn = SqlConnectionHelper.GetOpenConnection(connectionString))
            {
                var dr = conn.ExecuteReader(sqlCmd);
                while (dr.Read())
                {
                    string id = dr.IsDBNull(3) ? string.Empty : dr.GetValue(3).ToString();
                    string displayName = dr.IsDBNull(3) ? string.Empty : dr.GetValue(3).ToString();
                    string name = dr.IsDBNull(3) ? string.Empty : dr.GetValue(3).ToString();
                    string dataType = "";
                    string defaultValue = "";
                    string isNullable = "Y";
                    string length = "";
                    string identity = string.Empty;
                    string comment = "主键";

                    Column column = new Column(id, displayName, name, dataType, comment);
                    column.Length = DataTypeConvertHelper.ToInt(length);
                    column.IsAutoIncremented = false;
                    column.IsNullable = isNullable.Equals("Y");
                    column.DefaultValue = defaultValue;
                    column.DataType = dataType;
                    column.OriginalName = name;
                    column.TableName = dr.IsDBNull(0) ? string.Empty : dr.GetString(0);
                    columns.Add(column);
                }
                dr.Close();

                return columns;
            }
        }
        #endregion
    }
}
