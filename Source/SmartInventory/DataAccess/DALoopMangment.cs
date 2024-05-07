using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{

    public class DALoopMangment : Repository<NELoopDetails>
    //public class DALoopMangment : Repository<NELoopCables>
    {



        private static DALoopMangment objCable = null;
        private static readonly object lockObject = new object();
        public static DALoopMangment Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objCable == null)
                    {
                        objCable = new DALoopMangment();
                    }
                }
                return objCable;
            }
        }

        public List<NELoopDetails> GetLoopDetails(double longitude, double latitude, int associated_SystemId, string associated_System_Type, int structure_id)
        {
            try
            {
                return repo.ExecuteProcedure<NELoopDetails>("fn_get_loop_details", new { p_longitude = longitude, p_latitude = latitude, p_associated_system_id = associated_SystemId, p_associated_system_type = associated_System_Type, p_structure_id = structure_id }, true);
            }
            catch { throw; }
        }

        public NELoopDetails UpdateLoopDetails(int associated_system_id, string associated_entity_type, string associated_network_id, List<NELoopDetails> lstLoop, NetworkCodeIn objIn, int userId = 0)
        {
            NELoopDetails resultItem = new NELoopDetails();
            List<NELoopDetails> ListLoopDetailsInsert = lstLoop.Where(p => p.system_id == 0 && p.loop_length != null && p.loop_length > 0).ToList();
            List<NELoopDetails> ListLoopDetailsUpdate = lstLoop.Where(p => p.system_id != 0 && p.loop_length > 0).ToList();
            if (ListLoopDetailsInsert.Count > 0)
            {
                foreach (var item in ListLoopDetailsInsert)
                {
                    item.created_by = userId;
                    item.created_on = DateTimeHelper.Now;
                    item.associated_system_id = associated_system_id;
                    item.associated_entity_type = associated_entity_type;
                    item.associated_network_id = associated_network_id;
                    item.longitude = Convert.ToDouble(objIn.eGeom.Split(' ')[0]);
                    item.latitude = Convert.ToDouble(objIn.eGeom.Split(' ')[1]);

                    List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
                    objRegionProvince = DABuilding.Instance.GetRegionProvince(objIn.eGeom, GeometryType.Point.ToString());
                    if (objRegionProvince != null && objRegionProvince.Count > 0)
                    {
                        item.region_id = objRegionProvince[0].region_id;
                        item.province_id = objRegionProvince[0].province_id;
                        item.region_name = objRegionProvince[0].region_name;
                        item.province_name = objRegionProvince[0].province_name;
                    }

                    var networkCodeDetail = new DAMisc().GetNetworkCodeDetail(objIn);
                    if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
                    {
                        item.parent_entity_type = networkCodeDetail.parent_entity_type;
                        item.parent_network_id = networkCodeDetail.parent_network_id;
                        item.parent_system_id = networkCodeDetail.parent_system_id;
                        item.network_id = networkCodeDetail.network_code;
                        item.sequence_id = networkCodeDetail.sequence_id;
                    }

                    resultItem = repo.Insert(item);
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    resultItem.longitude = Convert.ToDouble(objIn.eGeom.Split(' ')[0]);
                    resultItem.latitude = Convert.ToDouble(objIn.eGeom.Split(' ')[1]);
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Loop.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(item.system_id, Models.EntityType.Loop.ToString(), item.province_id, 0);

                }
                //return resultItem;

            }
            else if (ListLoopDetailsUpdate.Count > 0)
            {
                ListLoopDetailsUpdate.ForEach(p => p.modified_by = userId);
                ListLoopDetailsUpdate.ForEach(p => p.modified_on = DateTimeHelper.Now);
                repo.Update(ListLoopDetailsUpdate);
                foreach (var item in ListLoopDetailsUpdate)
                {
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(item.system_id, Models.EntityType.Loop.ToString(), item.province_id, 1);
                }
            }

            return resultItem;
        }


        public int DeleteLoopDetailById(int system_id)
        {
            try
            {
                if (system_id > 0)
                {
                    return repo.Delete(system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }


        public List<NELoopDetails> GetLoopDetailsForCable(int cable_SystemId)
        {
            try
            {
                return repo.ExecuteProcedure<NELoopDetails>("fn_get_loop_details_for_cable", new { p_cable_system_id = cable_SystemId }, true);
            }
            catch { throw; }
        }

        public List<NECableDetails> GetNearByCableDetails(double longitude, double latitude, int bufferInMtrs)
        {
            try
            {
                return repo.ExecuteProcedure<NECableDetails>("fn_get_nearbycables", new { p_longitude = longitude, p_latitude = latitude, p_buffer = bufferInMtrs}, true);
            }
            catch { throw; }
        }


    }
    
}