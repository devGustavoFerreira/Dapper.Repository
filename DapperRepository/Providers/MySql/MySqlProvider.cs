using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace DapperRepository.Providers.MySql
{
    public class MySqlProvider : IProvider 
    {
        #region Constant
        private const string INSERT_QUERY = "INSERT INTO `{0}`({1}) VALUES(@{2}); SELECT Last_Insert_Id();";
        private const string INSERT_BULK_QUERY = "INSERT INTO `{0}`({1}) VALUES ({2});\r\n";
        private const string UPDATE_QUERY = "UPDATE `{0}` SET {1} WHERE `{0}`.`Id` = @Id";
        private const string UPDATE_BULK_QUERY = "UPDATE `{0}` SET {1} WHERE `{0}`.`Id` = @Id\r\n";
        private const string DELETE_QUERY = "DELETE FROM `{0}` WHERE `{0}`.`Id` = @Id";
        private const string DELETE_BULK_QUERY = "DELETE FROM `{0}` WHERE `{0}`.`Id` IN(@Ids)";
        private const string SELECT_QUERY = "SELECT\r\n {1} FROM `{0}`";
        private const string SELECT_FIRST_QUERY = "SELECT {1} FROM `{0}`\r\n";
        #endregion

        #region Fields
        private IDbConnection _connection;
        #endregion

        #region Utilities
        protected virtual IEnumerable<string> GetColumns(Type entityType)
        {
            PropertyInfo[] props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return props.Select(p => p.Name);
        }

        protected virtual IEnumerable<string> GetColumnsWithoutIdentity(Type entityType)
        {
            PropertyInfo[] props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return props.Where(p => p.Name != "Id").Select(p => p.Name);
        }
        #endregion

        #region Methods
        public virtual IDbConnection CreateConnection(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);

            return _connection;
        }

        public virtual string InsertQuery(string tableName, object entity)
        {
            IEnumerable<string> columns = GetColumnsWithoutIdentity(entity.GetType());

            return string.Format(INSERT_QUERY,
                                 tableName,
                                 string.Join(", ", columns.Select(p => string.Format("`{0}`.`{1}`", tableName, p))),
                                 string.Join(", @", columns));
        }

        public virtual string InsertBulkQuery(string tableName, IEnumerable<object> entities)
        {
            if (!entities.Any())
                throw new ArgumentException("collection is empty");

            IList<string> values = new List<string>();
            StringBuilder builder = new StringBuilder();
            IEnumerable<string> columns = GetColumnsWithoutIdentity(entities.First().GetType());
            string formattedColumns = string.Join(", ", columns.Select(p => string.Format("`{0}`.`{1}`", tableName, p)));

            for (int i = 0; i < entities.Count(); i++)
            {
                IEnumerable<string> valueColumns = columns.Select(p => string.Format("@{0}{1}", p, i + 1));
                builder.AppendFormat(INSERT_BULK_QUERY,
                                 tableName,
                                 formattedColumns,
                                 string.Join(", ", valueColumns));
            }

            return builder.ToString();
        }

        public virtual string UpdateQuery(string tableName, object entity)
        {
            IEnumerable<string> columns = GetColumnsWithoutIdentity(entity.GetType());
            string formattedColumns = string.Join(", ", columns.Select(p => string.Format("`{0}`.`{1}` = @{1}", tableName, p)));

            return string.Format(UPDATE_QUERY,
                                 tableName,
                                 formattedColumns);
        }

        public virtual string UpdateBulkQuery(string tableName, IEnumerable<object> entities)
        {
            if (!entities.Any())
                throw new ArgumentException("collection is empty");

            IList<string> values = new List<string>();
            object[] entityArray = entities.ToArray();

            StringBuilder builder = new StringBuilder();
            IEnumerable<string> columns = GetColumnsWithoutIdentity(entityArray[0].GetType());

            for (int i = 0; i < entityArray.Length; i++)
            {
                IEnumerable<string> formattedColumns = columns.Select(p => string.Format("`{0}`.`{1}` = @{1}{2}", tableName, p, i + 1));
                builder.AppendFormat(UPDATE_BULK_QUERY,
                                 tableName,
                                 string.Join(", ", formattedColumns));
            }

            return builder.ToString();
        }

        public virtual string DeleteQuery(string tableName)
        {
            return string.Format(DELETE_QUERY,
                                 tableName);
        }

        public virtual string DeleteBulkQuery(string tableName)
        {
            return string.Format(DELETE_BULK_QUERY,
                                 tableName);
        }

        public virtual string SelectQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity
        {
            IEnumerable<string> columns = GetColumns(typeof(T)).Select(p => string.Format("`{0}`.`{1}`", tableName, p));

            string query = string.Format(SELECT_QUERY,
                tableName,
                string.Join(",\r\n", columns));

            if (expression != null)
            {
                WhereBuilder translater = new WhereBuilder();
                string whereClasure = translater.Translate(expression);

                if (!string.IsNullOrEmpty(whereClasure))
                    query = string.Format("{0} WHERE {1}", query, whereClasure);
            }

            return query;
        }

        public virtual string SelectFirstQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity
        {
            IEnumerable<string> columns = GetColumns(typeof(T)).Select(p => string.Format("`{0}`.`{1}`", tableName, p));

            string query = string.Format(SELECT_FIRST_QUERY,
                tableName,
                string.Join(",\r\n", columns));

            if (expression != null)
            {
                WhereBuilder translater = new WhereBuilder();
                string whereClasure = translater.Translate(expression);

                if (!string.IsNullOrEmpty(whereClasure))
                    query = string.Format("{0} WHERE {1}", query, whereClasure);
            }

            return $"{query} LIMIT 1;";
        }
        #endregion
    }
}