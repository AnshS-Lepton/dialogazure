using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DARegionProvince : Repository<ViewRegionProvinces>
    {
        DARegionProvince()
        {

        }

        private static DARegionProvince objRegionProvince = null;
        private static readonly object lockObject = new object();
        public static DARegionProvince Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objRegionProvince == null)
                    {
                        objRegionProvince = new DARegionProvince();
                    }
                }
                return objRegionProvince;
            }
        }

        public List<Status> SaveRegionProvinceGeomatery(UpdateGeomtaryProperties objValue)
        {
            try
            {
                string geomvalue = objValue.geomtext.ToString();//.Replace("MULTIPOLYGON ","");

                //if (geomvalue.Contains("MULTIPOLYGON"))
                //{
                //    geomvalue = geomvalue.Replace("MULTIPOLYGON", "").Trim();
                //    geomvalue = geomvalue.Trim().Remove(0, 1) + "";
                //    geomvalue = geomvalue.Trim().Remove(geomvalue.Length - 1, 1) + "";
                //}

                //if (!geomvalue.Contains("MULTI") && geomvalue != "" && geomvalue.Substring(0, 4).ToUpper() == "POLY")//if (geomvalue.Contains("POLYGON"))
                //{
                //    geomvalue = geomvalue.Replace("POLYGON", "").Trim();
                //    geomvalue = geomvalue.Trim().Remove(0, 2) + "";
                //    geomvalue = geomvalue.Trim().Remove(geomvalue.Length - 2, 2) + "";
                //}

                var res = repo.ExecuteProcedure<Status>("fn_save_region_province_boundary", new { p_regionname = objValue.region_name, p_regionabbreviation = objValue.region_abbreviation, p_provincename = objValue.province_name, p_provinceabbreviation = objValue.province_abbreviation, p_countryname = objValue.country_name, p_boundarytype = objValue.boundarytype, p_geomtext = geomvalue.ToString(), p_shapefilepath = objValue.shapefilepath, p_userId = objValue.created_by, p_id = objValue.existing_id, p_action = objValue.entryStatus, p_is_active = objValue.is_active }).ToList();
                return res;
            }
            catch { throw; }
        }

        //public List<UpdateGeomtaryValue> SaveLogRegionProvince(UpdateGeomtaryValue objValue)
        //public List<UpdateGeomtaryValue> SaveLogRegionProvince(UpdateGeomtaryProperties objValue)
        //{
        //    try
        //    {
        //        var res = repo.ExecuteProcedure<UpdateGeomtaryValue>("fn_insert_regionprovince_logdetails", new { boundarytype = objValue.boundarytype.ToString(), username = objValue.username, filename = objValue.filename, useraction = objValue.useraction });
        //        return res;
        //    }
        //    catch { throw; }
        //}

        public string GetRegionProvinceFileNameById(int Id)
        {
            return repo.ExecuteProcedure<string>("fn_get_region_province_filename_by_id", new { province_id = Id }).FirstOrDefault();
            
        }
        public List<ViewRegionProvinces> GetRegionProvinceDetails()
        {
            return repo.GetAll().ToList();

        }
        public List<ViewRegionProvinces> GetRegionProvinceList(CommonGridAttributes objGridAttributes, string reporttype, string regprovfilter)
        {
            try
            {
                return repo.ExecuteProcedure<ViewRegionProvinces>("fn_get_regionProvince_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    P_REPORTTYPE = reporttype,
                    p_regprovfilter = regprovfilter
                }, true);
            }
            catch { throw; }
        }
        public List<InRegionByProvince> GetRegionDetailbyProvinceGeom(string geomtext)
        {
            try
            {
                return repo.ExecuteProcedure<InRegionByProvince>("fn_getregion_byprovince_geom", new
                {
                    p_geometry = geomtext
                });
            }
            catch { throw; }
        }
        public List<Status> DeleteRegionProvince(int id, string boundaryType, int user_Id)
        {
            try
            {
                var res = repo.ExecuteProcedure<Status>("fn_delete_region_province", new { p_id = id, p_boundarytype = boundaryType, p_userId = user_Id }).ToList();
                return res;
            }
            catch { throw; }
        }
        public DbMessage ValidateRegionProvinceBoundary(int system_id, string entity_type, string geom, string action, string region_name, string region_abbreviation, string province_name, string province_abbreviation, string country)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_region_province_geom_update", new { p_system_id = system_id, p_entity_type = entity_type, p_longlat = geom, p_action = action, p_regionname = region_name, p_regionabbreviation = region_abbreviation, p_provincename = province_name, p_provinceabbreviation = province_abbreviation, p_countryname = country }).FirstOrDefault();

            }
            catch(Exception ex) { throw ex; }
        }
        public DbMessage ValidateBoundary(int system_id, string entity_type)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_boundary", new { p_system_id = system_id, p_entity_type = entity_type}).FirstOrDefault();

            }
            catch { throw; }
        }
    }




    //public class DLRegionProvicneDetails : Repository<ViewRegionProvinces>
    //{
    //    //DLRegionProvicneDetails()
    //    //{

    //    //}
    //    //private static readonly object lockObject = new object();
    //    //private static DLRegionProvicneDetails objRegionProvinceDetails = null;
    //    //public static DLRegionProvicneDetails Instance
    //    //{

    //    //    get
    //    //    {
    //    //        lock (lockObject)
    //    //        {
    //    //            if (objRegionProvinceDetails == null)
    //    //            {
    //    //                objRegionProvinceDetails = new DLRegionProvicneDetails();
    //    //            }
    //    //        }
    //    //        return objRegionProvinceDetails;
    //    //    }
    //    //}



    //    public List<ViewRegionProvinces> GetRegionProvinceDetails()
    //    {
    //        return repo.GetAll().ToList();

    //    }
    //}
}
