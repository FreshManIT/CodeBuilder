using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.DataSource.Exporter
{
    using PhysicalDataModel;
    using Util;

    public class MySql5Exporter : BaseExporter
    {
        /// <summary>
        /// Rewrite interface info.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override Model Export(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder(connectionString);
            string originalDbName = connBuilder.Database;
            connBuilder.Database = "information_schema";

            Model model = new Model
            {
                Database = "MySQL5",
                Tables = GetTables(originalDbName, connBuilder.ConnectionString),
                Views = GetViews(originalDbName, connBuilder.ConnectionString)
            };

            return model;
        }

        /// <summary>
        /// Get MySQL Tables.
        /// </summary>
        /// <param name="originalDbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Tables GetTables(string originalDbName, string connectionString)
        {
            Tables tables = new Tables(10);

            string sqlCmd = string.Format("SELECT TABLE_NAME, TABLE_COMMENT FROM TABLES WHERE TABLE_SCHEMA = '{0}'", originalDbName);
            MySqlDataReader dr = MySqlHelper.ExecuteReader(connectionString, sqlCmd);
            while (dr.Read())
            {
                string id = dr.GetString(0);
                string displayName = dr.GetString(0);
                string name = dr.GetString(0);
                string comment = dr.IsDBNull(1) ? string.Empty : dr.GetString(1);

                Table table = new Table(id, displayName, name, comment)
                {
                    OriginalName = name,
                    Columns = GetColumns(displayName, originalDbName, connectionString),
                    PrimaryKeys = GetPrimaryKeys(displayName, originalDbName, connectionString),
                    ReferencedParent = GetReferencedParentList(connectionString, name),
                    ReferencedChild = GetReferencedChildList(connectionString, name)
                };
                tables.Add(id, table);
            }
            dr.Close();

            return tables;
        }

        /// <summary>
        /// Get views info.
        /// </summary>
        /// <param name="originalDbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Views GetViews(string originalDbName, string connectionString)
        {
            Views views = new Views(10);

            string sqlCmd = string.Format("SELECT TABLE_NAME FROM VIEWS WHERE TABLE_SCHEMA = '{0}'", originalDbName);
            MySqlDataReader dr = MySqlHelper.ExecuteReader(connectionString, sqlCmd);
            while (dr.Read())
            {
                string id = dr.GetString(0);
                string displayName = dr.GetString(0);
                string name = dr.GetString(0);
                string comment = string.Empty;

                View view = new View(id, displayName, name, comment)
                {
                    OriginalName = name,
                    Columns = GetColumns(displayName, originalDbName, connectionString)
                };
                views.Add(id, view);
            }
            dr.Close();

            return views;
        }

        /// <summary>
        /// Get tables columns.
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="originalDbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Columns GetColumns(string tableOrViewName, string originalDbName, string connectionString)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME,DATA_TYPE,COLUMN_KEY,COLUMN_DEFAULT, ");
            sqlBuilder.Append("IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH,EXTRA,COLUMN_COMMENT ");
            sqlBuilder.Append("FROM COLUMNS ");
            sqlBuilder.AppendFormat("WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME ='{1}' ", originalDbName, tableOrViewName);

            return GetColumns(connectionString, sqlBuilder.ToString());
        }

        /// <summary>
        /// GetKeys
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="originalDbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public Columns GetKeys(string tableName, string originalDbName, string connectionString)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME,DATA_TYPE,COLUMN_KEY,COLUMN_DEFAULT, ");
            sqlBuilder.Append("IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH,EXTRA,COLUMN_COMMENT ");
            sqlBuilder.Append("FROM COLUMNS ");
            sqlBuilder.AppendFormat("WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME ='{1}' AND CHARACTER_LENGTH(COLUMN_KEY)>0 ", originalDbName, tableName);

            return GetColumns(connectionString, sqlBuilder.ToString());
        }

        /// <summary>
        /// Get primary keys
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="originalDbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Columns GetPrimaryKeys(string tableName, string originalDbName, string connectionString)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME,DATA_TYPE,COLUMN_KEY,COLUMN_DEFAULT, ");
            sqlBuilder.Append("IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH,EXTRA,COLUMN_COMMENT ");
            sqlBuilder.Append("FROM COLUMNS ");
            sqlBuilder.AppendFormat("WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME ='{1}' AND COLUMN_KEY='PRI'", originalDbName, tableName);

            return GetColumns(connectionString, sqlBuilder.ToString());
        }

        /// <summary>
        /// Get columns info.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        private Columns GetColumns(string connectionString, string sqlCmd)
        {
            Columns columns = new Columns(50);
            MySqlDataReader dr = MySqlHelper.ExecuteReader(connectionString, sqlCmd);
            while (dr.Read())
            {
                string id = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                string displayName = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                string name = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                string dataType = dr.IsDBNull(3) ? string.Empty : dr.GetString(3);
                //string key = dr.IsDBNull(4) ? string.Empty : dr.GetString(4);
                string defaultValue = dr.IsDBNull(5) ? string.Empty : dr.GetString(5);
                string isNullable = dr.IsDBNull(6) ? string.Empty : dr.GetString(6);
                string length = dr.IsDBNull(7) ? string.Empty : dr.GetString(7);
                string identity = dr.IsDBNull(8) ? string.Empty : dr.GetString(8);
                string comment = dr.IsDBNull(9) ? string.Empty : dr.GetString(9);

                Column column = new Column(id, displayName, name, dataType, comment);
                column.Length = ConvertHelper.GetInt32(length);
                column.IsAutoIncremented = identity.Equals("auto_increment");
                column.IsNullable = isNullable.Equals("YES");
                column.DefaultValue = defaultValue.ToEmpty();
                column.DataType = dataType;
                column.OriginalName = name;
                columns.Add(id, column);
            }
            dr.Close();

            return columns;
        }

        /// <summary>
        /// get referenced parent list
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="tableName">you need table name</param>
        private List<ReferencedModel> GetReferencedParentList(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return null;
            string sqlCmd = $@"
SELECT
	CONSTRAINT_NAME ForeignKey,
	TABLE_NAME TableName,
	COLUMN_NAME ForeignKeyCell,
	REFERENCED_TABLE_NAME ReferencedTableName,
	REFERENCED_COLUMN_NAME ReferencedCell
FROM
	INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
	REFERENCED_TABLE_NAME = '{tableName}'";
            var resulteInfo = new List<ReferencedModel>();
            MySqlDataReader dr = MySqlHelper.ExecuteReader(connectionString, sqlCmd);
            while (dr.Read())
            {
                var tempModel = new ReferencedModel
                {
                    ForeignKey = (string)dr["ForeignKey"],
                    ForeignKeyCell = (string)dr["ForeignKeyCell"],
                    ReferencedCell = (string)dr["ReferencedCell"],
                    ReferencedTableName = (string)dr["ReferencedTableName"],
                    TableName = (string)dr["TableName"]
                };
                resulteInfo.Add(tempModel);
            }
            dr.Close();
            return resulteInfo;
        }

        /// <summary>
        /// get referenced parent list
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="tableName">you need table name</param>
        private List<ReferencedModel> GetReferencedChildList(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return null;
            string sqlCmd = $@"
SELECT
	CONSTRAINT_NAME ForeignKey,
	TABLE_NAME TableName,
	COLUMN_NAME ForeignKeyCell,
	REFERENCED_TABLE_NAME ReferencedTableName,
	REFERENCED_COLUMN_NAME ReferencedCell
FROM
	INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
	TABLE_NAME='{tableName}' and REFERENCED_TABLE_NAME !=''";
            var resulteInfo = new List<ReferencedModel>();
            MySqlDataReader dr = MySqlHelper.ExecuteReader(connectionString, sqlCmd);
            while (dr.Read())
            {
                var tempModel = new ReferencedModel
                {
                    ForeignKey = (string)dr["ForeignKey"],
                    ForeignKeyCell = (string)dr["ForeignKeyCell"],
                    ReferencedCell = (string)dr["ReferencedCell"],
                    ReferencedTableName = (string)dr["ReferencedTableName"],
                    TableName = (string)dr["TableName"]
                };
                resulteInfo.Add(tempModel);
            }
            dr.Close();
            return resulteInfo;
        }
    }
}
