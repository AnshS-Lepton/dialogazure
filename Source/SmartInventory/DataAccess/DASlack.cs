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
        public List<NEDuctDetails> GetNearByDuctDetails(double longitude, double latitude, int bufferInMtrs)
        {
            try
            {
                return repo.ExecuteProcedure<NEDuctDetails>("fn_get_nearbyducts", new { p_longitude = longitude, p_latitude = latitude, p_buffer = bufferInMtrs }, true);
            }
            catch { throw; }
        }
    }

}