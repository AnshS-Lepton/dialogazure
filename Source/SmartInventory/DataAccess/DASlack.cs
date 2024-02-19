using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccess
{

    public class DASlack : Repository<SlackMaster>
    {



        private static DASlack obj = null;
        private static readonly object lockObject = new object();
        public static DASlack Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (obj == null)
                    {
                        obj = new DASlack();
                    }
                }
                return obj;
            }
        }

        public List<SlackMaster> GetSlackDetails(double longitude, double latitude, int associated_SystemId, string associated_System_Type, int structure_id)
        {
            try
            {
                return repo.ExecuteProcedure<SlackMaster>("fn_get_slack_details", new { p_longitude = longitude, p_latitude = latitude, p_associated_system_id = associated_SystemId, p_associated_system_type = associated_System_Type, p_structure_id = structure_id }, true);
            }
            catch { throw; }
        }

        public SlackMaster UpdateLoopDetails(int associated_system_id, string associated_entity_type, string associated_network_id, List<SlackMaster> lstSlack, NetworkCodeIn objIn, int userId = 0)
        {
            SlackMaster resultItem = new SlackMaster();
            List<SlackMaster> ListSlackDetailsInsert = lstSlack.Where(p => p.system_id == 0 && p.slack_length != null && p.slack_length > 0).ToList();
            List<SlackMaster> ListSlackDetailsUpdate = lstSlack.Where(p => p.system_id != 0 && p.slack_length > 0).ToList();
            if (ListSlackDetailsInsert.Count > 0)
            {
                foreach (var item in ListSlackDetailsInsert)
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
                    geom.entityType = EntityType.Slack.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);

                }
                //return resultItem;

            }
            else if (ListSlackDetailsUpdate.Count > 0)
            {
                ListSlackDetailsUpdate.ForEach(p => p.modified_by = userId);
                ListSlackDetailsUpdate.ForEach(p => p.modified_on = DateTimeHelper.Now);
                repo.Update(ListSlackDetailsUpdate);
            }

            return resultItem;
        }


        public int DeleteSlackDetailById(int system_id)
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


        public List<SlackMaster> GetSlackDetailsForDuct(int duct_system_id)
        {
            try
            {
                return repo.ExecuteProcedure<SlackMaster>("fn_get_slack_details_for_duct", new { p_duct_system_id = duct_system_id }, true);
            }
            catch { throw; }
        }
        public PageMessage SaveEntitySlack(string Slacks)
        {
            try
            {
                return repo.ExecuteProcedure<PageMessage>("fn_save_slack_details", new { p_Slacks = Slacks }).FirstOrDefault(); ;

            }
            catch { throw; }
        }
    }

}