using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLRegionProvince
    {
        public BLRegionProvince()
        {

        }
        private static BLRegionProvince objRegionProvince = null;
        private static readonly object lockObject = new object();
        public static BLRegionProvince Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objRegionProvince == null)
                    {
                        objRegionProvince = new BLRegionProvince();
                    }
                }
                return objRegionProvince;
            }
        }



        public List<Status> SaveRegionProvinceGeomatery(UpdateGeomtaryProperties objInfo)
        {
            return DARegionProvince.Instance.SaveRegionProvinceGeomatery(objInfo);
        }

        //public List<UpdateGeomtaryValue> SaveLogRegionProvince(UpdateGeomtaryProperties objInfo)
        //{
        //    return DARegionProvince.Instance.SaveLogRegionProvince(objInfo);
        //}
        public string GetRegionProvinceFileNameById(int provinceId)
        {
            return DataAccess.DARegionProvince.Instance.GetRegionProvinceFileNameById(provinceId);
        }
        
        public List<ViewRegionProvinces> GetRegionProvinceDetails()
        {
            return DataAccess.DARegionProvince.Instance.GetRegionProvinceDetails();
        }
        public List<ViewRegionProvinces> GetRegionProvinceList(CommonGridAttributes objGridAttributes, string reporttype,string regprovfilter)
        {
            return DataAccess.DARegionProvince.Instance.GetRegionProvinceList(objGridAttributes, reporttype, regprovfilter);
        }
        public List<InRegionByProvince> GetRegionDetailbyProvinceGeom(string geomtext)
        {
            return DataAccess.DARegionProvince.Instance.GetRegionDetailbyProvinceGeom(geomtext);
        }
        public List<Status> DeleteRegionProvince(int id,string boundaryType,int user_Id)
        {
            return DataAccess.DARegionProvince.Instance.DeleteRegionProvince(id, boundaryType, user_Id);
        }
        public DbMessage ValidateRegionProvinceBoundary(int system_id, string entity_type, string geom, string action, string region_name, string region_abbreviation, string province_name, string province_abbreviation, string country)
        {
            return DataAccess.DARegionProvince.Instance.ValidateRegionProvinceBoundary(system_id, entity_type, geom, action, region_name, region_abbreviation, province_name, province_abbreviation, country);
        }
        public DbMessage ValidateBoundary(int system_id, string entity_type)
        {
            return DataAccess.DARegionProvince.Instance.ValidateBoundary(system_id, entity_type);
        }
        

    }
}