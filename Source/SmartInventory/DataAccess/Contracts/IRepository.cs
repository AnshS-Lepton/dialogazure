using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Contracts
{
        public interface IRepository<T> where T : class
        {
            IEnumerable<T> GetAll();
            IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);
            IEnumerable<T> GetAll(Expression<Func<T, object>> sortExpression, string order);
            IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, int maximumRows, int startRowIndex);
            IEnumerable<T> GetAll(Expression<Func<T, object>> sortExpression, string order, int maximumRows, int startRowIndex);
            IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sortExpression, string order, int maximumRows, int startRowIndex);
            IEnumerable<T> GetAll(string sqlQuery);
            DataTable GetDataTable(string sqlQuery);
            T Get(Expression<Func<T, bool>> predicate);
            T GetById(object Id);
            T Insert(T obj);
            void Insert(List<T> obj);
            bool Exists(object primaryKey);
            int Delete(object Id);
            int DeleteRange(List<T> obj);
            int Delete(T obj);
            T Update(T obj);
            int Update(List<T> obj);
        }
}
