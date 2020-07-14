using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DapperRepository
{
    public interface IDataContext
    {
        #region Sync Methods
        void Insert<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        int InsertBulk<T>(IEnumerable<T> items, IDbTransaction transaction = null) where T : BaseEntity;
        int Update<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        int Delete<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        int DeleteBulk<T>(IEnumerable<T> items, IDbTransaction transaction = null) where T : BaseEntity;
        T Find<T>(int Id) where T : BaseEntity;
        T Find<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        IEnumerable<T> FindAll<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        int Execute(string commandText, object parameters = null, IDbTransaction transaction = null);
        IDataReader ExecuteReader(string commandText, object parameters = null, IDbTransaction transaction = null);
        T ExecuteScalar<T>(string commandText, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity;
        int ExecuteProcedure(string storedProcedureName, object parameters = null, IDbTransaction transaction = null);
        IDataReader ExecuteReaderProcedure(string storedProcedureName, object parameters = null, IDbTransaction transaction = null);
        T ExecuteScalarProcedure<T>(string storedProcedureName, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity;
        #endregion

        #region Async Methods
        Task InsertAsync<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        Task<int> InsertBulkAsync<T>(IEnumerable<T> items, IDbTransaction transaction = null) where T : BaseEntity;
        Task<int> UpdateAsync<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        Task<int> DeleteAsync<T>(T item, IDbTransaction transaction = null) where T : BaseEntity;
        Task<int> DeleteBulkAsync<T>(IEnumerable<T> items, IDbTransaction transaction = null) where T : BaseEntity;
        Task<T> FindAsync<T>(int Id) where T : BaseEntity;
        Task<T> FindAsync<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        Task<IEnumerable<T>> FindAllAsync<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        Task<int> ExecuteAsync(string commandText, object parameters = null, IDbTransaction transaction = null);
        Task<IDataReader> ExecuteReaderAsync(string commandText, object parameters = null, IDbTransaction transaction = null);
        Task<T> ExecuteScalarAsync<T>(string commandText, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity;
        Task<int> ExecuteProcedureAsync(string storedProcedureName, object parameters = null, IDbTransaction transaction = null);
        Task<IDataReader> ExecuteReaderProcedureAsync(string storedProcedureName, object parameters = null, IDbTransaction transaction = null);
        Task<T> ExecuteScalarProcedureAsync<T>(string storedProcedureName, object parameters = null, IDbTransaction transaction = null) where T : BaseEntity;
        #endregion

        IDbConnection Connection { get; }
        void OpenConnection();
        IDbTransaction BeginTransaction();
    }
}