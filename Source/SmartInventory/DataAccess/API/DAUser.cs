using DataAccess.DBHelpers;
using Models;
using System;

namespace DataAccess.API
{
    public class DL_API_User : Repository<User>, System.IDisposable
    {
        public User ValidateUser(string username, string password)
        {
            try
            {
                return repo.Get(u => u.user_name.ToLower() == username.ToLower() && u.password == password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public User getUserDetails(int id)
        {
            try
            {
                return repo.Get(u => u.user_id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public User ChkUserExist(string username)
        {
            try
            {
                return repo.Get(u => u.user_name.ToLower() == username.ToLower() || u.user_email.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Dispose() { }
    }
}
