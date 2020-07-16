using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DapperRepository.Providers
{
    public interface IProvider
    {
        IDbConnection CreateConnection();
        string InsertQuery(string tableName, object entity);
        string InsertBulkQuery(string tableName, IEnumerable<object> entities);
        string UpdateQuery(string tableName, object entity);
        string UpdateBulkQuery(string tableName, IEnumerable<object> entities);
        string DeleteQuery(string tableName);
        string DeleteBulkQuery(string tableName);
        string SelectQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity;
        string SelectFirstQuery<T>(Expression<Func<T, bool>> expression, string tableName) where T : BaseEntity;
    }
}