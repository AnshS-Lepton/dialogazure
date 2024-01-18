using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public class BLProvinceBoundary
    {
       DAProvince ObjDAProvinceBoundary = new DAProvince();
       public ProvinceBoundary ChkProvinceNameExist(string ProvinceName)
       {
           return ObjDAProvinceBoundary.ChkProvinceNameExist(ProvinceName);
       }
        public ProvinceBoundary getProvinceInfo(int provinceId) 
        {
            return ObjDAProvinceBoundary.getProvinceInfo(provinceId);
        }
    }
}
