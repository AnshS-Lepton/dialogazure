using DataAccess.DBHelpers;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DARegionProvince : Repository<UpdateGeomtaryValue>
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
                string geomvalue = objValue.geomtext.ToString();
             
                //if (geomvalue.Contains("MULTIPOLYGON"))
                //{
                //    geomvalue = geomvalue.Replace("MULTIPOLYGON", "");

                //    geomvalue = geomvalue.Trim().Remove(0, 1) + "";
                //    geomvalue = geomvalue.Trim().Remove(geomvalue.Length - 1, 1) + "";

                //}

                //if (geomvalue.Contains("POLYGON"))
                //{
                //    geomvalue = geomvalue.Replace("POLYGON", "");
                   
                //}

                var res = repo.ExecuteProcedure<Status>("fn_insertupdate_boundary_geom", new { provincename = objValue.provincename.ToString(), regionname = objValue.region_name.ToString(), boundarytype = objValue.boundarytype, geomtext = geomvalue.ToString(), shapefilepath = objValue.shapefilepath, userId = objValue.created_by }).ToList();
                return res;
                
            
            }
            catch { throw; }
        }

        //public List<UpdateGeomtaryValue> SaveLogRegionProvince(UpdateGeomtaryValue objValue)
        public List<UpdateGeomtaryValue> SaveLogRegionProvince(UpdateGeomtaryProperties objValue)
        {
            try
            {
                var res = repo.ExecuteProcedure<UpdateGeomtaryValue>("fn_insert_regionprovince_logdetails", new { boundarytype = objValue.boundarytype.ToString(), username = objValue.username, filename = objValue.filename, useraction = objValue.useraction });
                return res;
            }
            catch { throw; }
        }

        
    }
}
