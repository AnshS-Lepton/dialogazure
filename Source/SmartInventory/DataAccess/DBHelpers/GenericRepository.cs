using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
namespace DataAccess.DBHelpers
{
    //REPOSITORY DESING PATTERN
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private DbContext context;
        // represent respective table to perform certain operations  
        private DbSet<T> dbSet;
        string errorMessage = string.Empty;

        public GenericRepository(DbContext _context)
        {
            context = _context;
            context.Database.CommandTimeout = 18000;            
            dbSet = context.Set<T>();
        }

        /// Get all the records from table
        /// </summary>
        /// <returns>a list of type of current class</returns>
        public IEnumerable<T> GetAll()
        {
            using (context)
            {
                return dbSet.ToList();
            }
        }
        /// <summary>
        /// Get all the records from table accroding to where clause 
        /// </summary>
        /// <param name="predicate">a=>a.columnName=="value"</param>
        /// <returns></returns> /// <returns>a list of type of current class</returns>
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            using (context)
            {
                return dbSet.Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Get all the records from table with sorting 
        /// </summary>
        /// <param name="sortExpression">a=>a.columnName</param>
        /// <param name="order">DESC or ASC default ASC</param>
        /// <returns>a list of type of current class</returns>
        public IEnumerable<T> GetAll(Expression<Func<T, object>> sortExpression, string order)
        {
            using (context)
            {
                if (order == "DESC")
                    return dbSet.OrderByDescending(sortExpression).ToList();
                else
                    return dbSet.OrderBy(sortExpression).ToList();
            }
        }


        /// <summary>
        /// Get all the records from table accroding to where clause with sorting and limit from starting with a index position
        /// </summary>
        /// <param name="predicate">a=>a.columnName=="value"</param>
        /// <param name="sortExpression">a=>a.columnName</param>
        /// <param name="order">DESC or ASC default ASC</param>
        /// <param name="maximumRows">how many row you want to extract from datatable</param>
        /// <param name="startRowIndex">where you want to start like 1 or 10</param>
        /// <returns>a list of type of current class</returns>
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, int maximumRows, int startRowIndex)
        {
            using (context)
            {
                return dbSet.Where(predicate).Skip<T>(startRowIndex).Take(maximumRows).ToList();
            }
        }


        /// <summary>
        /// Get all the records from table accroding to where clause with sorting and limit from starting with a index position
        /// </summary>
        /// <param name="predicate">a=>a.columnName=="value"</param>
        /// <param name="sortExpression">a=>a.columnName</param>
        /// <param name="order">DESC or ASC default ASC</param>
        /// <param name="maximumRows">how many row you want to extract from datatable</param>
        /// <param name="startRowIndex">where you want to start like 1 or 10</param>
        /// <returns>a list of type of current class</returns>
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sortExpression, string order, int maximumRows, int startRowIndex)
        {
            using (context)
            {
                if (order == "DESC")
                    return dbSet.Where(predicate).OrderByDescending(sortExpression).Skip<T>(startRowIndex).Take(maximumRows).ToList();
                else
                    return dbSet.Where(predicate).OrderBy(sortExpression).Skip<T>(startRowIndex).Take(maximumRows).ToList();
            }
        }

        /// <summary>
        /// Get all the records from table with sorting and limit from starting with a index position
        /// </summary>
        /// <param name="sortExpression">a=>a.columnName</param>
        /// <param name="order">DESC or ASC default ASC</param>
        /// <param name="maximumRows">how many row you want to extract from datatable</param>
        /// <param name="startRowIndex">where you want to start like 1 or 10</param>
        /// <returns>a list of type of current class</returns>
        public IEnumerable<T> GetAll(Expression<Func<T, object>> sortExpression, string order, int maximumRows, int startRowIndex)
        {
            using (context)
            {
                if (order == "DESC")
                    return dbSet.OrderByDescending(sortExpression).Skip<T>(startRowIndex).Take(maximumRows).ToList();
                else
                    return dbSet.OrderBy(sortExpression).Skip<T>(startRowIndex).Take(maximumRows).ToList();
            }
        }

        public IEnumerable<T> GetAll(string sqlQuery)
        {
            if (sqlQuery == null)
                return null;
            using (context)
            {
                return dbSet.SqlQuery(sqlQuery).ToList();
            }
        }
        /// <summary>
        /// Execute Procedure or Sql query on database
        /// </summary>
        /// <param name="sqlQuery">select * from user_master</param>
        /// <returns>return a list of object class</returns>
        public DataTable GetDataTable(string sqlQuery)
        {

            DataTable dt = new DataTable();

            if (sqlQuery == null)
                return dt;
            var conn = context.Database.Connection;
            var connectionState = conn.State;
            try
            {
                using (context)
                {
                    if (connectionState != ConnectionState.Open)
                        conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = CommandType.Text;
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connectionState != ConnectionState.Open)
                    conn.Close();
            }
            return dt;


            // return context.Database.SqlQuery<object>(sqlQuery).ToList();
        }

        /// <summary>
        /// To get a single record from datatable
        /// </summary>
        /// <param name="sortExpression">a=>a.columnName</param>
        /// <returns>current class object</returns>
        public T Get(Expression<Func<T, bool>> predicate)
        {
            using (context)
            {
                return dbSet.FirstOrDefault(predicate);
            }
        }

        /// <summary>
        /// To get a single record from datatable based on primary key value
        /// </summary>
        /// <param name="id">primary key value</param>
        /// <returns>current class object</returns>
        public T GetById(object id)
        {
            if (id == null)
                return null;
            using (context)
            {
                return dbSet.Find(id);
            }
        }
        public T GetById(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions)
        {
            using (context)
            {
                IQueryable<T> set = dbSet;
                if (includeExpressions.Any())
                {
                    foreach (var includeExpression in includeExpressions)
                    {
                        set = set.Include(includeExpression);
                    }

                    return set.Where(predicate).FirstOrDefault();
                }

                return dbSet.Where(predicate).FirstOrDefault();
            }
        }
        /// <summary>
        /// Insert single record in current table
        /// </summary>
        /// <param name="obj">object of current class</param>
        /// <returns> saved object</returns>
        public T Insert(T obj)
        {
            if (obj == null)
                return null;
            using (context)
            {
                dbSet.Add(obj);
                Save();
                return obj;
            }
        }


        /// <summary>
        /// To Insert multiple records in current table
        /// </summary>
        /// <param name="obj">List of current table's objects</param>
        /// <returns>1 if successfull</returns>
        public void Insert(List<T> obj)
        {
            if (obj != null)
                using (context)
                {
                    dbSet.AddRange(obj);
                    Save();

                }
        }

        /// <summary>
        /// To check a record exists in datatable
        /// </summary>
        /// <param name="primaryKey">primary key value</param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            if (primaryKey == null)
                return false;
            using (context)
            {
                return dbSet.Find(primaryKey) == null ? false : true;
            }
        }

        /// <summary>
        /// To Delete a record from current table
        /// </summary>
        /// <param name="id">Primary key value</param>
        public virtual int Delete(object id)
        {
            int retval = 0;
            if (id != null)
                using (context)
                {
                    T entityToDelete = dbSet.Find(id);
                    retval = Delete(entityToDelete);
                }
            return retval;
        }

        /// <summary>
        /// To Delete Current object's record from current datatable
        /// </summary>
        /// <param name="entityToDelete"></param>
        public int Delete(T entityToDelete)
        {
            int retval = 0;
            if (entityToDelete != null)
                using (context)
                {
                    if (context.Entry(entityToDelete).State == EntityState.Detached)
                    {
                        dbSet.Attach(entityToDelete);
                        //it add the row like in particular table, particular field, particular value has been deleted with timestamps.  
                    }
                    dbSet.Remove(entityToDelete);
                    Save();
                    retval = 1;
                }
            return retval;
        }

        public int DeleteRange(List<T> obj)
        {
            int retval = 0;
            if (obj != null)
                using (context)
                {
                    foreach (var entity in obj)
                    {
                        if (context.Entry(entity).State == EntityState.Detached)
                            dbSet.Attach(entity);
                    }

                    dbSet.RemoveRange(obj);
                    Save();
                    retval = 1;
                }
            return retval;
        }

        /// <summary>
        /// To Update record in datatable
        /// </summary>
        /// <param name="obj">Current class object</param>
        /// <returns></returns>
        public T Update(T obj)
        {
            if (obj == null)
                return null;
            using (context)
            {
                dbSet.Attach(obj);
                context.Entry(obj).State = EntityState.Modified;
                //same to record the modified state and where, what and when  
                Save();
                return obj;
            }
        }

        /// <summary>
        /// To Update record in datatable
        /// </summary>
        /// <param name="obj">Current class object</param>
        /// <returns></returns>
        public int Update(List<T> obj)
        {
            if (obj == null)
                return 0;
            using (context)
            {
                foreach (var item in obj)
                {
                    dbSet.Attach(item);
                    context.Entry(item).State = EntityState.Modified;
                }
                //same to record the modified state and where, what and when  
                Save();
                return 1;
            }
        }
        //Save changes to database
        public virtual void Save()
        {
            try
            {
                context.SaveChanges();
                // to keep changing the entityframe as well db  
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(errorMessage, dbEx);
            }
        }

        //Execute procedure with return type...
        ///GENERIC METHOD TO EXECUTE ANY RETUNR TYPE POSTGRES PROCEDURE..
        /// <typeparam name="T"> RETURN TYPE CLASS NAME</typeparam>
        /// <param name="ProcName">PROCEDURE/FUNCTION NAME</param>
        /// <param name="inputParams"> INPUT CLASS OBJECT</param>
        /// <param name="isProcReturnJson">THIS PARAMETER IS TO DEFINE THAT WHETHER PROCEDURE RETURN JSON OR OBJECT</param>
        public List<TEntity> ExecuteProcedure<TEntity>(string ProcName, object inputParams, bool isProcReturnJson = false)
        {
            using (context)
            {
                var finalQuery = "";
                List<TEntity> lstResult = new List<TEntity>();
                var finalParams = ProcHelper.GetInputParamsWithFinalQuery(ProcName, inputParams, ref finalQuery);
                if (isProcReturnJson)
                {
                    var lstJSONResult = context.Database.SqlQuery<string>(finalQuery, finalParams.ToArray()).ToList();
                    return ProcHelper.ConvertJsonToObject<TEntity>(lstJSONResult);
                }
                else
                {
                    return lstResult = context.Database.SqlQuery<TEntity>(finalQuery, finalParams.ToArray()).ToList();
                }
            }
        }

        public void ExecuteProcedure(string ProcName, object inputParams)
        {
            using (context)
            {
                var finalQuery = "";
                List<T> lstResult = new List<T>();
                var finalParams = ProcHelper.GetInputParamsWithFinalQuery(ProcName, inputParams, ref finalQuery);
                context.Database.ExecuteSqlCommand(finalQuery, finalParams.ToArray());
            }
        }
        // this dispose method after very instance  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }
        public int ExecuteSQLCommand(string sqlQuery)
        {

            return context.Database.ExecuteSqlCommand(sqlQuery);
        }
       public string connectionString { get; set; }
    }
}
