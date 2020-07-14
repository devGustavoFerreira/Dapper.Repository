using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DapperRepository
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Insert(T item);
        void InsertBulk(IEnumerable<T> items);
        void Update(T item);
        void Delete(T item);
        void DeleteBulk(IEnumerable<T> items);
        T Find(int Id);
        T Find(Expression<Func<T, bool>> expression);
        IList<T> FindAll(Expression<Func<T, bool>> expression);
    }
}