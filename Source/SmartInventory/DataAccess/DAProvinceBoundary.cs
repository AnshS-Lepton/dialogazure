using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataAccess
{
    public class DAProvince : Repository<ProvinceBoundary>
    {
       
        public ProvinceBoundary ChkProvinceNameExist(string ProvinceName)
        {
            try
            {
                return repo.Get(u => u.province_name.ToLower() == ProvinceName.ToLower());
            }
            catch
            {
                throw;
            }
        }
        public ProvinceBoundary getProvinceInfo(int provinceId)
        {
            try
            {
                var result= repo.Get(u => u.id == provinceId);
                return result != null ? result : new ProvinceBoundary();
            }
            catch
            {
                throw;
            }
        }
    }


    
}
