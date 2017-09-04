using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CodeBuilder.PhysicalDataModel;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.DataSource.Exporter
{
    public class SqlServer2005Exporter : BaseExporter
    {
        #region IExporter Members

        public override Model Export(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            Model model = new Model
            {
                Database = "SqlServer2005",
                Tables = GetTables(connectionString),
                Views = GetViews(connectionString)
            };

            return model;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Get tables.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Tables GetTables(string connectionString)
        {
            Tables tables = new Tables(10);

            string sqlCmd = "select [name],[object_id] from sys.tables where type='U'";
            SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, CommandType.Text, sqlCmd);
            while (dr.Read())
            {
                string id = dr.GetString(0);
                string displayName = dr.GetString(0);
                string name = dr.GetString(0);
                string comment = string.Empty;
                int objectId = dr.GetInt32(1);

                Table table = new Table(id, displayName, name, comment)
                {
                    OriginalName = name,
                    Columns = GetColumns(objectId, connectionString),
                    ReferencedParent = GetReferencedParentList(connectionString, name),
                    ReferencedChild = GetReferencedChildList(connectionString, name)
                };
                table.PrimaryKeys = GetPrimaryKeys(objectId, connectionString, table.Columns);
                tables.Add(id, table);
            }
            dr.Close();

            return tables;
        }

        /// <summary>
        /// Get views.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Views GetViews(string connectionString)
        {
            Views views = new Views(10);

            string sqlCmd = "select [name],[object_id] from sys.views where type='V'";
            SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, CommandType.Text, sqlCmd);
            while (dr.Read())
            {
                string id = dr.GetString(0);
                string displayName = dr.GetString(0);
                string name = dr.GetString(0);
                string comment = string.Empty;
                int objectId = dr.GetInt32(1);

                View view = new View(id, displayName, name, comment)
                {
                    OriginalName = name,
                    Columns = GetColumns(objectId, connectionString)
                };
                views.Add(id, view);
            }
            dr.Close();

            return views;
        }

        /// <summary>
        /// Get columns.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Columns GetColumns(int objectId, string connectionString)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("select c.object_id,c.column_id,c.name,c.max_length,c.is_identity,c.is_nullable,c.is_computed,");
            sqlBuilder.Append("t.name as type_name,p.value as description,d.definition as default_value ");
            sqlBuilder.Append("from sys.columns as c ");
            sqlBuilder.Append("inner join sys.types as t on c.user_type_id =  t.user_type_id ");
            sqlBuilder.Append("left join sys.extended_properties as p on p.major_id = c.object_id and p.minor_id = c.column_id ");
            sqlBuilder.Append("left join sys.default_constraints as d on d.parent_object_id = c.object_id and d.parent_column_id = c.column_id ");
            sqlBuilder.AppendFormat("where c.object_id={0}", objectId);

            return GetColumns(connectionString, sqlBuilder.ToString());
        }

        /// <summary>
        /// get primary keys.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="connectionString"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private Columns GetPrimaryKeys(int objectId, string connectionString, Columns columns)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("select syscolumns.name from syscolumns,sysobjects,sysindexes,sysindexkeys ");
            sqlBuilder.AppendFormat("where syscolumns.id ={0} ", objectId);
            sqlBuilder.Append("and sysobjects.xtype = 'PK' and sysobjects.parent_obj = syscolumns.id ");
            sqlBuilder.Append("and sysindexes.id = syscolumns.id and sysobjects.name = sysindexes.name and ");
            sqlBuilder.Append("sysindexkeys.id = syscolumns.id and sysindexkeys.indid = sysindexes.indid and syscolumns.colid = sysindexkeys.colid");

            Columns primaryKeys = new Columns(4);
            SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, CommandType.Text, sqlBuilder.ToString());
            while (dr.Read())
            {
                string name = dr.IsDBNull(0) ? string.Empty : dr.GetString(0);
                if (columns.ContainsKey(name)) primaryKeys.Add(name, columns[name]);
            }
            dr.Close();

            return primaryKeys;
        }

        /// <summary>
        /// Get columns.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        private Columns GetColumns(string connectionString, string sqlCmd)
        {
            Columns columns = new Columns(50);
            SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, CommandType.Text, sqlCmd);
            while (dr.Read())
            {
                string id = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                string displayName = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                string name = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                int length = dr.IsDBNull(3) ? 0 : dr.GetInt16(3);
                bool identity = !dr.IsDBNull(4) && dr.GetBoolean(4);
                bool isNullable = !dr.IsDBNull(5) && dr.GetBoolean(5);
                bool isComputed = !dr.IsDBNull(6) && dr.GetBoolean(6);
                string dataType = dr.IsDBNull(7) ? string.Empty : dr.GetString(7);
                string comment = dr.IsDBNull(8) ? string.Empty : dr.GetString(8);
                string defaultValue = dr.IsDBNull(9) ? string.Empty : dr.GetString(9);

                Column column = new Column(id, displayName, name, dataType, comment);
                column.Length = length;
                column.IsAutoIncremented = identity;
                column.IsNullable = isNullable;
                column.DefaultValue = defaultValue;
                column.DataType = dataType;
                column.OriginalName = name;
                column.IsComputed = isComputed;
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

    object_name(constraint_object_id) ForeignKey,
	object_name(parent_object_id) TableName,
	col_name(
        parent_object_id,
        parent_column_id
    ) ForeignKeyCell,
	object_name(referenced_object_id) ReferencedTableName,
	col_name(
        referenced_object_id,
        referenced_column_id
    ) ReferencedCell
FROM

    sys.foreign_key_columns
WHERE
    referenced_object_id = object_id('{tableName}')";
            var resulteInfo = new List<ReferencedModel>();
            SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, CommandType.Text, sqlCmd);
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

    object_name(constraint_object_id) ForeignKey,
	object_name(parent_object_id) TableName,
	col_name(
        parent_object_id,
        parent_column_id
    ) ForeignKeyCell,
	object_name(referenced_object_id) ReferencedTableName,
	col_name(
        referenced_object_id,
        referenced_column_id
    ) ReferencedCell
FROM

    sys.foreign_key_columns
WHERE
    parent_object_id=OBJECT_ID('{tableName}')";
            var resulteInfo = new List<ReferencedModel>();
            SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, CommandType.Text, sqlCmd);
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
        #endregion
    }
}
