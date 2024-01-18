
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBContext;
using DataAccess.DBHelpers;
using Models;
using Models.WFM;
using Npgsql;
using Utility;

namespace DataAccess.WFM
{
    public class DLWFMTicket : Repository<SlotRequest>
    {
        public List<Models.ticketStepDetails> getTicketStepDetails(string job_id)
        {
            try
            {
                return repo.ExecuteProcedure<Models.ticketStepDetails>("fn_api_wfm_ticket_steps", new { job_id = job_id }, true);
                //return result != null && result.Count > 0 ? result : new List<ticketStepDetails>(); 
            }
            catch { throw; }
        }

        public List<Models.ticketStepDetails> getTTTicketStepDetails(string job_id)
        {
            try
            {
                return repo.ExecuteProcedure<Models.ticketStepDetails>("fn_api_wfm_ticket_steps_tt", new { job_id = job_id }, true);
                //return result != null && result.Count > 0 ? result : new List<ticketStepDetails>(); 
            }
            catch { throw; }
        }
        public int  InsertSlotRequest(SlotRequest obj)
        {
            try
            {
                var entity = repo.Get(b => b.referenceid == obj.referenceid);
                if (entity != null)
                {                    
                    return Convert.ToInt32(-1);
                }
                else
                {
                    repo.Insert(obj);
                    return Convert.ToInt32(obj.id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int UpdateSlotRequesNew(SlotRequest obj)
        {
            try
            {
                var entity = repo.Get(b => b.referenceid == obj.referenceid);
                if (entity != null)
                {
                    entity.response = obj.response;
                    entity.managerid = obj.managerid;
                    entity.remark = obj.remark;
                    entity.request = obj.request;
                    repo.Update(entity);
                    return Convert.ToInt32(-1);
                }
                else
                {
                    //repo.Insert(obj);
                    return Convert.ToInt32(-1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int updateSlotRequest(SlotRequest obj)
        {
            try
            {
                var entity = repo.Get(b => b.referenceid == obj.referenceid);
                if (entity != null)
                {

                    entity.order_type = obj.order_type;
                    entity.service_type = obj.service_type;
                    entity.latitude = obj.latitude;
                    entity.longitude = obj.longitude;
                    entity.appointment_date = obj.appointment_date;
                    entity.jo_category = obj.jo_category;
                    entity.request = obj.request;
                    repo.Update(entity);
                    return Convert.ToInt32(entity.id);
                }
                else
                {
                    return Convert.ToInt32(-1);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public SlotRequest GetSlotRequest(string refereanceid)
        {
            try
            {
                SlotRequest result = new SlotRequest();
                var entity = repo.GetAll(b => b.referenceid == refereanceid);
                if (entity != null)
                {
                    result = entity.FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int updateSlotRequestAppmntDate(string referenceid, DateTime appointment_date)
        {
            try
            {
                var entity = repo.Get(b => b.referenceid == referenceid);
                if (entity != null)
                {
                    entity.appointment_date = appointment_date;
                    repo.Update(entity);
                    return Convert.ToInt32(entity.id);
                }
                else
                {
                    return Convert.ToInt32(-1);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static List<ManagerList> GetManagerList(double lat, double lng, string role_name = "WFM Contractor")
        {
            //List<ManagerList> ManagerList = new List<ManagerList>()
            //{
            //    new ManagerList(){ user_id=1,user_name="Manager1"}, // return only one manager
            //    //new ManagerList(){ UserId=101,Name="Manager2"},
            //};

            List<ManagerList> ManagerList = new List<ManagerList>();
            //2
            //using (MainContext context = new MainContext())
            //{
            //    //string query = string.Format(@"SELECT um.* from province_boundary pb
            //    //join user_permission_area upa on pb.id = upa.province_id
            //    //join user_master um on um.user_id = upa.user_id
            //    //join role_master rm on rm.role_id = um.role_id
            //    //where
            //    //ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and rm.role_name = 'WFM Contractor'", lng, lat);

            //    string query = string.Format(@"SELECT um.* from province_boundary pb
            //    join user_permission_area upa on pb.id = upa.province_id
            //    join user_master um on um.user_id = upa.user_id
            //    join role_master rm on rm.role_id = um.role_id
            //    where
            //    ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and rm.role_name = {2}", lng, lat, role_name);

            //    ManagerList = context.Database.SqlQuery<ManagerList>(query).ToList();
            //}

            //3
            using (MainContext context = new MainContext())
            {
                //string query = string.Format(@"SELECT um.* from province_boundary pb
                //join user_permission_area upa on pb.id = upa.province_id
                //join user_master um on um.user_id = upa.user_id
                //join role_master rm on rm.role_id = um.role_id
                //where
                //ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and rm.role_name = 'WFM Contractor'", lng, lat);


                ////filter for province boundary
                //string query = string.Format(@"SELECT sftm.service_facility_name as service,jtm.jo_name as jo_type,
                //usftm.service_facility_name as user_service,ujtm.jo_name as user_Jo_type,
                //um.* from province_boundary pb
                //join user_permission_area upa on pb.id = upa.province_id
                //join user_master um on um.user_id = upa.user_id
                //join role_master rm on rm.role_id = um.role_id

                //left join wfm_role_jo_type_mapping tm on tm.role_id=rm.role_id
                //left join wfm_jo_type_master jtm on jtm.id=tm.jo_type_id
                //left join wfm_role_service_facility_mapping fm on fm.role_id=rm.role_id
                //left join wfm_service_facility_master sftm on sftm.id=fm.service_facility_id

                //left join wfm_user_jo_type_mapping utm on utm.user_id=um.user_id
                //left join wfm_jo_type_master ujtm on ujtm.id=utm.jo_type_id
                //left join wfm_user_service_facility_mapping ufm on ufm.user_id=um.user_id
                //left join wfm_service_facility_master usftm on usftm.id=ufm.service_facility_id
                //where
                //ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and rm.role_name = '{2}'", lng, lat, role_name);
                //ManagerList = context.Database.SqlQuery<ManagerList>(query).ToList();

                //Filter for block boundary

                //string query = string.Format(@"SELECT sftm.service_facility_name as service,jtm.jo_name as jo_type,
                //usftm.service_facility_name as user_service,ujtm.jo_name as user_Jo_type,
                //um.* from tbl_block_boundary pb
                //join user_permission_area upa on pb.id = upa.block_id
                //join user_master um on um.user_id = upa.user_id
                //join role_master rm on rm.role_id = um.role_id

                //left join wfm_role_jo_type_mapping tm on tm.role_id=rm.role_id
                //left join wfm_jo_type_master jtm on jtm.id=tm.jo_type_id
                //left join wfm_role_service_facility_mapping fm on fm.role_id=rm.role_id
                //left join wfm_service_facility_master sftm on sftm.id=fm.service_facility_id

                //left join wfm_user_jo_type_mapping utm on utm.user_id=um.user_id
                //left join wfm_jo_type_master ujtm on ujtm.id=utm.jo_type_id
                //left join wfm_user_service_facility_mapping ufm on ufm.user_id=um.user_id
                //left join wfm_service_facility_master usftm on usftm.id=ufm.service_facility_id
                //where
                //ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and rm.role_name = '{2}'", lng, lat, role_name);
                //ManagerList = context.Database.SqlQuery<ManagerList>(query).ToList();

                //string query = string.Format(@"SELECT sftm.service_facility_name as service,jtm.jo_name as jo_type,
                //usftm.service_facility_name as user_service,ujtm.jo_name as user_Jo_type,
                //jcm1.jo_category_name as jo_category_role,jcm2.jo_category_name as jo_category_user,
                //um.* from tbl_block_boundary pb
                //join user_permission_area upa on pb.id = upa.block_id
                //join user_master um on um.user_id = upa.user_id
                //join role_master rm on rm.role_id = um.role_id

                //left join wfm_role_jo_type_mapping tm on tm.role_id=rm.role_id
                //left join wfm_jo_type_master jtm on jtm.id=tm.jo_type_id
                //left join wfm_role_service_facility_mapping fm on fm.role_id=rm.role_id
                //left join wfm_service_facility_master sftm on sftm.id=fm.service_facility_id

                //left join wfm_user_jo_type_mapping utm on utm.user_id=um.user_id
                //left join wfm_jo_type_master ujtm on ujtm.id=utm.jo_type_id
                //left join wfm_user_service_facility_mapping ufm on ufm.user_id=um.user_id
                //left join wfm_service_facility_master usftm on usftm.id=ufm.service_facility_id

                //left join wfm_role_jo_category_mapping rjm1 on rjm1.role_id=rm.role_id
                //left join wfm_jo_category_master jcm1 on jcm1.id=rjm1.jo_category_id
                //left join wfm_user_jo_category_mapping ujm2 on ujm2.user_id=um.user_id
                //left join wfm_jo_category_master jcm2 on jcm2.id=ujm2.jo_category_id
                //where
                //ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and rm.role_name = '{2}'", lng, lat, role_name);
                //ManagerList = context.Database.SqlQuery<ManagerList>(query).ToList();


                //Remove role validation
                string query = string.Format(@"SELECT pb.block_name,rm.role_name,coalesce(CAST(um.capacity AS int),0) as capacity,
                usftm.service_facility_code as user_service,ujtm.jo_code as user_Jo_type,
                jcm2.jo_category_code as jo_category_user,
                um.* from tbl_block_boundary pb
                left join user_permission_area upa on pb.id = upa.block_id
                left join user_master um on um.user_id = upa.user_id
                left join role_master rm on rm.role_id = um.role_id
                left join wfm_user_jo_type_mapping utm on utm.user_id=um.user_id
                left join wfm_jo_type_master ujtm on ujtm.id=utm.jo_type_id
                left join wfm_user_service_facility_mapping ufm on ufm.user_id=um.user_id
                left join wfm_service_facility_master usftm on usftm.id=ufm.service_facility_id
                left join wfm_user_jo_category_mapping ujm2 on ujm2.user_id=um.user_id
                left join wfm_jo_category_master jcm2 on jcm2.id=ujm2.jo_category_id
                where
                ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and um.is_active=true and (rm.role_name = '{2}' or rm.role_name is null)", lng, lat, role_name);
                ManagerList = context.Database.SqlQuery<ManagerList>(query).ToList();


            }


            return ManagerList;
        }

        //public static List<FEList> GetFEList(int ManagerUserId)
        //{
        //    List<FEList> FEList = new List<FEList>()
        //    {
        //        new FEList(){ UserId=469,Name="FE1",ManagerUserId=1},
        //        new FEList(){ UserId=1001,Name="FE2",ManagerUserId=100},
        //    };
        //    return FEList;
        //}
        public static List<ManagerList> GetAllManagerList(double lat, double lng, string role_name = "WFM Contractor")
        {
            

            List<ManagerList> ManagerList = new List<ManagerList>();
          
            using (MainContext context = new MainContext())
            {
               
                //Remove role validation
                string query = string.Format(@"SELECT pb.block_name,rm.role_name,coalesce(CAST(um.capacity AS int),0) as capacity,
                usftm.service_facility_code as user_service,ujtm.jo_code as user_Jo_type,
               COALESCE(jcm2.jo_category_code, '') as jo_category_user,
                um.* from tbl_block_boundary pb
                left join user_permission_area upa on pb.id = upa.block_id
                left join user_master um on um.user_id = upa.user_id
                left join role_master rm on rm.role_id = um.role_id
                left join wfm_user_jo_type_mapping utm on utm.user_id=um.user_id
                left join wfm_jo_type_master ujtm on ujtm.id=utm.jo_type_id
                left join wfm_user_service_facility_mapping ufm on ufm.user_id=um.user_id
                left join wfm_service_facility_master usftm on usftm.id=ufm.service_facility_id
                left join wfm_user_jo_category_mapping ujm2 on ujm2.user_id=um.user_id
                left join wfm_jo_category_master jcm2 on jcm2.id=ujm2.jo_category_id
                where
                ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and um.is_active=true and (rm.role_name In ('{2}') or rm.role_name is null)", lng, lat, role_name);
                ManagerList = context.Database.SqlQuery<ManagerList>(query).ToList();


            }


            return ManagerList;
        }

        public static string GetBlockName(double lat, double lng)
        {
           string blockName=string.Empty;
            using (MainContext context = new MainContext())
            {
               
                //Get Block Name basis on lat and log
                string query = string.Format(@"Select block_name from tbl_block_boundary
                where ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) limit 1", lng, lat);
                blockName = context.Database.SqlQuery<string>(query).FirstOrDefault();
            }
            return blockName;
        }

        public static bool isTimeSheetDefined(int userId, DateTime date_check_in, int day_of_week, string strTime, out string errorMessage) //ValidateLeaveDetail(Leave_Details objLeaveDetail, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                bool status = false;
                //using (MainContext context = new MainContext())
                //{
                //    //strTime = "09:30";
                //    string query = string.Format("select * from fn_wfm_validate_user_timesheet({0},'{1}','{2}','{3}')", userId, date_check_in.ToString("dd-MMM-yyyy"), day_of_week, strTime);
                //    errorMessage = context.Database.SqlQuery<string>(query).FirstOrDefault();
                //    status = errorMessage.Equals("success") ? true : false;
                //}

                try
                {
                    List<RosterVW> User_TimeSheet = new List<RosterVW>();
                    using (MainContext context = new MainContext())
                    {
                        var rUserId = new NpgsqlParameter("@p_user_id", userId);
                        var rdate_check_in = new NpgsqlParameter("@p_date", date_check_in.ToString("dd-MMM-yyyy"));
                        var r_day_of_week = new NpgsqlParameter("@p_day_of_week", day_of_week.ToString());
                        var r_time = new NpgsqlParameter("@p_time", strTime.ToString());

                        var parameters = new NpgsqlParameter[4]
                        {
                            rUserId,rdate_check_in,r_day_of_week,r_time
                        };
                        User_TimeSheet = DbHelper.ExecutePostgresProcedure<RosterVW>(context, "fn_wfm_validate_user_roster", parameters);
                        // return User_TimeSheet;
                    }
                }
                catch
                {
                    throw;
                }




                return status;
            }
            catch
            {
                return false;
                throw;
            }

        }


        public static RosterVW GetUserTimeSheet(int userId, DateTime date_check_in, int day_of_week, string strTime) //ValidateLeaveDetail(Leave_Details objLeaveDetail, out string errorMessage)
        {
            RosterVW User_TimeSheet = new RosterVW();
            try
            {
                using (MainContext context = new MainContext())
                {
                    var rUserId = new NpgsqlParameter("@p_user_id", userId);
                    var rdate_check_in = new NpgsqlParameter("@p_date", date_check_in.ToString("dd-MMM-yyyy"));
                    var r_day_of_week = new NpgsqlParameter("@p_day_of_week", day_of_week.ToString());
                    var r_time = new NpgsqlParameter("@p_time", strTime.ToString());

                    var parameters = new NpgsqlParameter[4]
                    {
                            rUserId,rdate_check_in,r_day_of_week,r_time
                    };
                    User_TimeSheet = DbHelper.ExecutePostgresProcedure<RosterVW>(context, "fn_wfm_validate_user_roster", parameters).FirstOrDefault();
                    return User_TimeSheet;
                }

            }
            catch
            {
                return User_TimeSheet;
                throw;
            }

        }
        public static List<EFSkill> GetSkillFE(string skillid)
        {
            // skillid = "3,4";
            List<EFSkill> EFSkillList = new List<EFSkill>()
            {

                 new EFSkill(){ UserId=1001,SkillId="1"},
                new EFSkill(){ UserId=1002,SkillId="1"},
                new EFSkill(){ UserId=1003,SkillId="2"},
                new EFSkill(){ UserId=1004,SkillId="2"},
                new EFSkill(){ UserId=1005,SkillId="1"},
                new EFSkill(){ UserId=1005,SkillId="2"},
                new EFSkill(){ UserId=1006,SkillId="3"},
                new EFSkill(){ UserId=1006,SkillId="4"},

                //new EFSkill(){ UserId=463,SkillId="1"},
                //new EFSkill(){ UserId=464,SkillId="1"},
                //new EFSkill(){ UserId=1003,SkillId="2"},
                //new EFSkill(){ UserId=1004,SkillId="2"},
                //new EFSkill(){ UserId=1005,SkillId="1"},
                //new EFSkill(){ UserId=1005,SkillId="2"},
                //new EFSkill(){ UserId=1006,SkillId="3"},
                //new EFSkill(){ UserId=1006,SkillId="4"},

                //new EFSkill(){ UserId=1001,SkillId="1"},
                //new EFSkill(){ UserId=1001,SkillId="2"},
                //new EFSkill(){ UserId=1002,SkillId="2"},
            };
            List<EFSkill> result = new List<EFSkill>();
            EFSkillList.Where(a => skillid.Contains(a.SkillId)).Select(s => s.UserId).Distinct().ToList().ForEach(f =>
            {
                result.Add(new EFSkill() { UserId = f });
            });
            return result;
        }

        public static List<FEList> GetAVFE(List<FEList> FEList, List<EFSkill> SKFEList)
        {
            var q = (from x in FEList join y in SKFEList on x.UserId equals y.UserId select x);
            return q.ToList();
        }
        public static void GetTaskTracking(int ticketid, out DateTime? checkout, out DateTime? checkin, out User u)
        {
            using (MainContext context = new MainContext())
            {
                Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.hpsm_ticketid == ticketid).FirstOrDefault();
                checkin = taskTracking.mobile_checkin_time;
                checkout = taskTracking.mobile_checkout_time;
                u = context.User.Where(s => s.user_id == taskTracking.frt_id).FirstOrDefault();
                u.name = MiscHelper.Decrypt(u.name);
            }
        }

        public static string GetCountryName(string stateprovince)
        {
            using (MainContext context = new MainContext())
            {
                string query = string.Format("select rb.country_name from region_boundary rb join province_boundary pb on pb.region_id = rb.id where lower(pb.province_name)='{0}';", stateprovince.ToLower());
                var contury = context.Database.SqlQuery<string>(query).FirstOrDefault();
                return contury;
            }
        }

        public static string GetSchedule(string hpsmid)
        {
            using (MainContext context = new MainContext())
            {
                string query = string.Format("select * from fn_get_slot_time({0});", hpsmid);
                return context.Database.SqlQuery<string>(query).ToList()[0];
            }
        }

        public static List<material_used> GetMaterialUsed(string jobid)
        {
            using (MainContext context = new MainContext())
            {
                string query = string.Format("select * from vw_get_additional_material_used where jobid='{0}'", jobid);
                return context.Database.SqlQuery<material_used>(query).ToList();
            }
        }
        public static PortManager getPortManager(string stateorprovince)
        {
            using (MainContext context = new MainContext())
            {
                PortManager portManager = context.portmanagers.Where(m => m.stateorprovince == stateorprovince && m.is_active == true).FirstOrDefault();
                return portManager;
            }
        }
        public bool UpdateNapDetails(napdetails obj)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var entity = context.task.Where(b => b.hpsmid == obj.job_id).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.node = obj.nap;
                        entity.nap_port = obj.napport;
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





    }
    public class DLWFMUpdateNotificationLog : Repository<UpdateNotificationLog>
    {
        public int UpdateNotificationLog(napportnotificationdata obj)
        {
            int res = 0;
            try
            {
                UpdateNotificationLog objemail = new UpdateNotificationLog
                {
                    address = obj.to_email,
                    content = obj.subject_line + obj.email_message,
                    created_by = obj.user_id,
                    created_on = DateTime.Now,
                    notification_type = "email",
                    response = obj.email_response,
                    status = obj.email_result,
                    ticket_id = obj.ticket_id,
                };
                UpdateNotificationLog objsms = new UpdateNotificationLog
                {
                    address = Convert.ToString(obj.to_mobile),
                    content = obj.sms_message,
                    created_by = obj.user_id,
                    created_on = DateTime.Now,
                    notification_type = "sms",
                    response = obj.sms_response,
                    status = obj.sms_result,
                    ticket_id = obj.ticket_id,
                };
                repo.Insert(objemail);
                res++;
                repo.Insert(objsms);
                res++;
                return res;
            }
            catch { throw; }
        }
    }

    public class DLWFMServiceType : Repository<ServiceType>
    {
        public ServiceType Getservicetype(string serviceType)
        {
            try
            {
                var entity = repo.Get(b => b.name == serviceType);
                return entity;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
    public class DLWFMJobOrderDetail : Repository<Models.WFM.Task>
    {

        public int newEditTimeSheet(Models.WFM.Task obj)
        {

            try
            {
                var orderObjt = repo.Get(b => b.hpsmid == obj.hpsmid);
                if (orderObjt != null)
                {

                    orderObjt.cpe_brand = obj.cpe_brand;
                    orderObjt.cpe_model = obj.cpe_model;
                    orderObjt.cpe_serialno = obj.cpe_serialno;

                    repo.Update(orderObjt);
                    return orderObjt.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch { throw; }

        }


        public dynamic getTriggerFinalDetails(string jobId)
        {

            try
            {

                var entity = repo.GetAll(b => b.hpsmid == jobId).FirstOrDefault();
                string cpereturn = "";
                if(string.IsNullOrEmpty(entity.is_cpe_collected))
                {
                    
                    dynamic inputRequest =
                    new
                    {
                        orderId = (entity.parent_hpsmid).Split('-')[0],
                        //orderId = "OO870026",
                        atomicOrderId = entity.atomicidclose
                       
                    };
                    return inputRequest;
                }
                else
                {
                    cpereturn = entity.is_cpe_collected;
                    dynamic inputRequest =
                    new
                    {
                        orderId = (entity.parent_hpsmid).Split('-')[0],
                        //orderId = "OO870026",
                        atomicOrderId = entity.atomicidclose,
                        cpeReturnedFlag = cpereturn
                    };
                    return inputRequest;
                }
               
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int UpdateJobOrderDetail(JobOrderDetail obj)
        {

            try
            {
                var orderObj = repo.Get(b => b.hpsmid == obj.job_id);
                if (orderObj != null)
                {
                    if (obj.stage == 1)
                    {
                        orderObj.customer_rmn = obj.primary_contact;
                        orderObj.secondary_contact = obj.secondary_contact;
                        orderObj.email_id = obj.email_id;
                        orderObj.addressid = obj.address_id;
                        orderObj.addressline1 = obj.address_line1;
                        orderObj.addressline2 = obj.address_line2;
                        orderObj.addressline3 = obj.address_line3;
                        orderObj.addresstype = obj.address_type;
                        orderObj.city = obj.city;
                        orderObj.pincode = obj.pinCode;
                        orderObj.stateorprovince = obj.state_Province;
                        orderObj.latitude = obj.latitude;
                        orderObj.longitude = obj.longitude;
                    }
                    else if (obj.stage == 2)
                    {
                        orderObj.nap_port = obj.nap_port;
                        orderObj.node = obj.node;
                        orderObj.cpe_brand = obj.cpe_brand;
                        orderObj.cpe_model = obj.cpe_model;
                        orderObj.cpe_portno = obj.cpe_portno;
                        orderObj.cpe_serialno = obj.cpe_serialno;
                        orderObj.cpe_type = obj.cpe_type;

                        orderObj.cpe_ref_serial = obj.cpe_ref_serial;
                        orderObj.cpe_item_code = obj.cpe_item_code;
                        orderObj.cpe_uom = obj.cpe_uom;
                        orderObj.cpe_wh = obj.cpe_wh;
                        orderObj.device_serial_number1 = obj.set_up_box_serial_number;
                    }

                    //update if db stage is less then the current stage
                    //   if (Convert.ToInt32(orderObj.stage) < obj.stage)
                    //    {
                    orderObj.stage = obj.stage;
                    //    }
                    repo.Update(orderObj);
                    return orderObj.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch { throw; }

        }
        public int UpdateSendToERP(Models.WFM.Task obj)
        {

            try
            {
                var orderObj = repo.Get(b => b.hpsmid == obj.hpsmid);
                if (orderObj != null)
                {
                    
                    orderObj.sendtoerp= obj.sendtoerp;
                    orderObj.erp_response= obj.erp_response;
                    repo.Update(orderObj);
                    return orderObj.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch { throw; }

        }
        public int UpdatettStatus(getttStatusIn obj)
        {

            try
            {
                var orderObj = repo.Get(b => b.hpsmid == obj.job_id);
                if (orderObj != null)
                {
                    //  orderObj.earlystart = obj.main_issue_type;
                    orderObj.cls = obj.main_issue_type;
                    repo.Update(orderObj);
                    return orderObj.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch { throw; }

        }

        public customer_detail getCustomerDetail(string jobId)
        {
            try
            {
                customer_detail obj = new customer_detail();
                var entity = repo.GetAll(b => b.hpsmid == jobId).FirstOrDefault();
                if (entity != null)
                {
                    obj.address_id = entity.addressid;
                    obj.address_line1 = entity.addressline1;
                    obj.address_line2 = entity.addressline2;
                    obj.address_line3 = entity.addressline3;
                    obj.address_type = entity.addresstype;
                    obj.city = entity.city;
                    obj.email_id = entity.email_id;
                    obj.job_id = entity.hpsmid;
                    obj.latitude = entity.latitude;
                    obj.longitude = entity.longitude;
                    obj.pinCode = entity.pincode;
                    obj.primary_contact = entity.customer_rmn;
                    obj.secondary_contact = entity.secondary_contact;
                    obj.state_Province = entity.stateorprovince;
                    obj.customer_name = entity.subscriber_name;
                    return obj;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public cpe_detail getCPEDetail(string jobId)
        {
            try
            {
                cpe_detail obj = new cpe_detail();
                var entity = repo.GetAll(b => b.hpsmid == jobId).FirstOrDefault();
                if (entity != null)
                {
                    using (MainContext context = new MainContext())
                    {
                        List<BrandModel> objDetail = new List<BrandModel>();
                        BrandModel result = new BrandModel();
                        string query = string.Format(@"select model_name,description,is_active,created_datetime from tbl_wfm_model where upper(brand_name)=upper('{0}')", entity.cpe_brand);

                        obj.cpe_brand = entity.cpe_brand;
                        obj.cpe_model = entity.cpe_model;
                        obj.cpe_portno = entity.cpe_portno;
                        obj.cpe_serialno = entity.cpe_serialno;
                        obj.cpe_type = entity.cpe_type;
                        obj.job_id = entity.hpsmid;
                        obj.nap = entity.node;
                        obj.nap_port = entity.nap_port;
                        obj.cpe_ref_serial = entity.cpe_ref_serial;
                        obj.cpe_item_code = entity.cpe_item_code;
                        obj.cpe_uom = entity.cpe_uom;
                        obj.cpe_wh = entity.cpe_wh;
                        obj.set_up_box_serial_number = entity.device_serial_number1;
                        obj.is_cpe_collected = entity.is_cpe_collected;
                        obj.lstModels = objDetail = context.Database.SqlQuery<BrandModel>(query).ToList();
                        return obj;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GetValue(string slotid, out DateTime appointment_date, out int master_slot_id)
        {
            string year = slotid.Substring(0, 4);
            string month = slotid.Substring(4, 2);
            string day = slotid.Substring(6, 2);

            master_slot_id = slotid.Length == 10 ? Convert.ToInt32(slotid.Substring(8, 2)) : Convert.ToInt32(slotid.Substring(8, 1));
            appointment_date = Convert.ToDateTime(year + "-" + month + "-" + day);

        }
        public int UpdateCpeCollected(string hpsmid, string is_cpe_collected)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var orderObj = repo.Get(b => b.hpsmid == hpsmid);
                    if (orderObj != null)
                    {
                        orderObj.is_cpe_collected = is_cpe_collected;
                        repo.Update(orderObj);
                        return orderObj.hpsm_ticket_id;
                    }
                    else 
                    {
                        return Convert.ToInt32(-1);
                    }
                }
            }
            catch { throw; }
        }
        public int UpdatePaymentDetail(string hpsmid, string ar_no,string payment_mode,string payment_status)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var orderObj = repo.Get(b => b.hpsmid == hpsmid);
                    if (orderObj != null)
                    {
                        orderObj.ar_no = ar_no;
                        orderObj.payment_mode = payment_mode;
                        orderObj.payment_status = payment_status;
                        repo.Update(orderObj);
                        return orderObj.hpsm_ticket_id;
                    }
                    else
                    {
                        return Convert.ToInt32(-1);
                    }
                }
            }
            catch { throw; }
        }
        public int UpdateStatus(job_order_status obj)
        {
            try
            {

                using (MainContext context = new MainContext())
                {
                    tbl_wfm_jobstatus objDetail = new tbl_wfm_jobstatus();
                    string query = string.Format(@"select id,action,status,sub_status,is_active from tbl_wfm_jobstatus where UPPER(action)=UPPER('{0}')", obj.action);
                    objDetail = context.Database.SqlQuery<tbl_wfm_jobstatus>(query).FirstOrDefault();



                    var orderObj = repo.Get(b => b.hpsmid == obj.job_id);
                    if (orderObj != null)
                    {
                        orderObj.hpsmstatus = objDetail.status;
                        orderObj.status = objDetail.sub_status;
                        orderObj.ticketstatus = objDetail.action;
                        //orderObj.service_status = orderObj.service_status;
                        if (obj.action.ToUpper() == "CPE_ACTIVATE")
                        {
                            orderObj.service_status = obj.service_status;
                            orderObj.isresolve = "1";
                        }
                        else if (obj.action.ToUpper() == "CPE_ACTIVATION_FAIL")
                        {
                            orderObj.isresolve = "2";
                        }
                        else if(obj.action.ToUpper()== "IPTV_ACTIVATE")
                        {
                            orderObj.isresolve = "1";
                        }


                        if (!string.IsNullOrEmpty(obj.latitude))
                            orderObj.latitude = obj.latitude;
                        if (!string.IsNullOrEmpty(obj.cpe_serialno))
                            orderObj.cpe_serialno = obj.cpe_serialno;
                        if (!string.IsNullOrEmpty(obj.longitude))
                            orderObj.longitude = obj.longitude;
                        if (!string.IsNullOrEmpty(obj.rc))
                            orderObj.root_cause_id = obj.rc;
                        if (!string.IsNullOrEmpty(obj.rca))
                            orderObj.resolution_close_id = obj.rca;

                        if (!(obj.action.ToUpper() == "ACKNOWLEDGE" || obj.action.ToUpper() == "CHECK-IN"))
                            if (!string.IsNullOrEmpty(obj.remarks))
                                if(obj.action.ToUpper() != "COMPLETED")                              
                                orderObj.remarks = obj.remarks;
                        if ((obj.action.ToUpper() == "CAPTURE_ISSUE"))
                            if (!string.IsNullOrEmpty(obj.remarks))
                                orderObj.wfmcomment= obj.remarks;

                        if (obj.action.ToUpper() == "HOLD")
                        {
                            if (!string.IsNullOrEmpty(obj.rca))
                                orderObj.resolution_close_id = obj.rca;
                            var isSentMail = Convert.ToBoolean(ConfigurationManager.AppSettings["isSentMail"]);
                            if (isSentMail)
                            {
                                var template = (new DLWFMNotification()).GetTemplateDetail("ONHOLD");
                                if (template != null)
                                {
                                    //send mail
                                    var message = template.email;
                                    var isMailsent = Email.sendMail("", orderObj.email_id, template.subject, message);
                                    var id = (new DLWFMEmailsmsLog()).InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = orderObj.hpsmid, emailstatus = isMailsent == true ? 1 : 0, emaildeliverytime = DateTime.Now, emailremark = "", type = template.type });

                                    if (!string.IsNullOrEmpty(orderObj.customer_rmn))
                                    {
                                        string smsremark = "";
                                        int sentsms = SMS.SendSms(ref smsremark, orderObj.customer_rmn, message);
                                        (new DLWFMEmailsmsLog()).InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = orderObj.hpsmid, smsstatus = sentsms, smsdeliverytime = DateTime.Now, smsremark = smsremark, type = template.type });
                                    }
                                }
                            }
                        }

                        if (obj.action.ToUpper() == "CANCELLED")
                        {
                            if (!string.IsNullOrEmpty(obj.rca))
                                orderObj.resolution_close_id = obj.rca;
                            var isSentMail = Convert.ToBoolean(ConfigurationManager.AppSettings["isSentMail"]);
                            if (isSentMail)
                            {
                                var template = (new DLWFMNotification()).GetTemplateDetail("CANCELLED");
                                if (template != null)
                                {
                                    //send mail
                                    var message = template.email;
                                    var isMailsent = Email.sendMail("", orderObj.email_id, template.subject, message);

                                    var id = (new DLWFMEmailsmsLog()).InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = orderObj.hpsmid, emailstatus = isMailsent == true ? 1 : 0, emaildeliverytime = DateTime.Now, emailremark = "", type = template.type });

                                    if (!string.IsNullOrEmpty(orderObj.customer_rmn))
                                    {
                                        string smsremark = "";
                                        int sentsms = SMS.SendSms(ref smsremark, orderObj.customer_rmn, message);
                                        (new DLWFMEmailsmsLog()).InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = orderObj.hpsmid, smsstatus = sentsms, smsdeliverytime = DateTime.Now, smsremark = smsremark, type = template.type });
                                    }
                                }
                            }
                        }

                        repo.Update(orderObj);

                        Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.hpsm_ticketid == orderObj.hpsm_ticket_id).FirstOrDefault();
                        if (taskTracking != null)
                        {
                            taskTracking.status = objDetail.status;
                            taskTracking.sub_status = objDetail.sub_status;
                            taskTracking.modified_on = DateTime.Now;
                            if (!string.IsNullOrEmpty(obj.remarks))
                                taskTracking.remarks = obj.remarks;
                            if (obj.action.ToUpper() == "CHECK-IN")
                            {
                                taskTracking.mobile_checkin_time = DateTime.Now;
                            }
                            else if (obj.action.ToUpper() == "COMPLETED")
                            {
                                taskTracking.mobile_checkout_time = DateTime.Now;

                                if (obj.service_status !=null && obj.service_status.ToUpper() == "SEND BACK TO SOURCE")
                                {
                                    taskTracking.sub_status = obj.service_status;
                                }
                            }
                            context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        Route_Issue Route_Issue = context.Route_Issues.Where(s => s.hpsm_ticketid == orderObj.hpsm_ticket_id).FirstOrDefault();
                        if (Route_Issue != null)
                        {
                            Route_Issue.status = objDetail.status;
                            Route_Issue.sub_status = objDetail.sub_status;
                            context.Entry(Route_Issue).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                        return orderObj.hpsm_ticket_id;
                    }
                    else
                    {
                        return Convert.ToInt32(-1);
                    }
                }
            }
            catch { throw; }

        }
        public int UpdateReTrigger(job_order_status obj)
        {
            try
            {

                using (MainContext context = new MainContext())
                {
                    tbl_wfm_jobstatus objDetail = new tbl_wfm_jobstatus();
                    string query = string.Format(@"select id,action,status,sub_status,is_active from tbl_wfm_jobstatus where UPPER(action)=UPPER('{0}')", "Check-In");
                    objDetail = context.Database.SqlQuery<tbl_wfm_jobstatus>(query).FirstOrDefault();

                    var orderObj = repo.Get(b => b.hpsmid == obj.job_id);
                    if (orderObj != null)
                    {
                        orderObj.hpsmstatus = objDetail.status;
                        orderObj.status = objDetail.sub_status;
                        orderObj.ticketstatus = objDetail.action;
                        orderObj.stage = 0;
                        orderObj.remarks = "Re-Trigger";
                        repo.Update(orderObj);

                        Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.hpsm_ticketid == orderObj.hpsm_ticket_id).FirstOrDefault();
                        if (taskTracking != null)
                        {
                            taskTracking.status = objDetail.status;
                            taskTracking.sub_status = objDetail.sub_status;
                            taskTracking.modified_on = DateTime.Now;
                            taskTracking.remarks = "Re-Trigger";
                            context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        Route_Issue Route_Issue = context.Route_Issues.Where(s => s.hpsm_ticketid == orderObj.hpsm_ticket_id).FirstOrDefault();
                        if (Route_Issue != null)
                        {
                            Route_Issue.status = objDetail.status;
                            Route_Issue.sub_status = objDetail.sub_status;
                            context.Entry(Route_Issue).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                        return orderObj.hpsm_ticket_id;
                    }
                    else
                    {
                        return Convert.ToInt32(-1);
                    }
                }
            }
            catch { throw; }

        }

        public int UpdateStaticIPStatus(string hpsmid, string remark)
        {
            try
            {

                using (MainContext context = new MainContext())
                {

                    var orderObj = repo.Get(b => b.hpsmid == hpsmid);
                    if (orderObj != null)
                    {
                        orderObj.service_status = "Static IP activated";
                        orderObj.remarks = remark;
                        orderObj.isresolve = "1";
                        repo.Update(orderObj);
                        return orderObj.hpsm_ticket_id;
                    }
                    else
                    {
                        return Convert.ToInt32(-1);
                    }
                }
            }
            catch { throw; }

        }


        public string UpdateStatus_rch(job_order_status obj)
        {
            try
            {
                string msg = "";
                using (MainContext context = new MainContext())
                {
                    tbl_wfm_jobstatus objDetail = new tbl_wfm_jobstatus();
                    string query = string.Format(@"select id,action,status,sub_status,is_active from tbl_wfm_jobstatus where UPPER(action)=UPPER('{0}')", obj.action);
                    objDetail = context.Database.SqlQuery<tbl_wfm_jobstatus>(query).FirstOrDefault();

                    var orderObj = repo.Get(b => b.hpsmid == obj.job_id);
                    if (orderObj != null)
                    {
                        orderObj.hpsmstatus = objDetail.status;
                        orderObj.status = objDetail.sub_status;
                        orderObj.ticketstatus = objDetail.action;
                        if (!string.IsNullOrEmpty(obj.latitude))
                            orderObj.latitude = obj.latitude;
                        if (!string.IsNullOrEmpty(obj.cpe_serialno))
                            orderObj.cpe_serialno = obj.cpe_serialno;
                        if (!string.IsNullOrEmpty(obj.longitude))
                            orderObj.longitude = obj.longitude;
                        if (!(obj.action.ToUpper() == "ACKNOWLEDGE" || obj.action.ToUpper() == "CHECK-IN"))
                            if (!string.IsNullOrEmpty(obj.remarks))
                                orderObj.remarks = obj.remarks;

                        //For Re-Scheduled
                        if ((obj.action.ToUpper() == "Re-Scheduled".ToUpper()))
                        {

                            //if (obj.date != null)
                            //{
                            //orderObj.customer_preferred_time = obj.date;
                            //orderObj.earlystart = obj.time; //'10:00'
                            SlotConfirmation Request = (new DLWFMSlotConfirmation()).GetSlotConfirmation(orderObj.bookingid);
                            if (Request == null)
                            {
                                var message = string.Format("No record found in confirm slot");
                                return message;
                            }
                            //SlotRequest SlotRequest = (new DLWFMTicket()).GetSlotRequest(Request.referenceid);
                            //if (SlotRequest == null)
                            //{
                            //    var message = string.Format("No record found in slot request");
                            //    return message;
                            //}


                            //var service_types = SlotRequest.service_type;

                            //int slot_duration = 0;
                            //if (service_types.Contains(","))
                            //{
                            //    string[] services = service_types.Split(',');
                            //    foreach (var x in services)
                            //    {
                            //        var service_facility = (new DLWFMServiceFacilityMaster()).GetServiceFacility(x);
                            //        if (service_facility == null)
                            //        {
                            //            var message = string.Format("Service facility {0} not found", x);
                            //            return message;
                            //        }
                            //        slot_duration += service_facility.slot_duration;
                            //    }
                            //}
                            //else
                            //{
                            //    var service_facility = (new DLWFMServiceFacilityMaster()).GetServiceFacility(service_types);
                            //    if (service_facility == null)
                            //    {
                            //        var message = string.Format("Service facility {0} not found", service_types);
                            //        return message;
                            //    }
                            //    slot_duration = service_facility.slot_duration;
                            //}


                            //if (slot_duration == 0)
                            //{
                            //    var message = string.Format("Slot duration not map with service facility.");
                            //    return message;
                            //}
                            //var slotDuration = (new DLWFMSlotDuration()).GetSlotDurationDetails(slot_duration);

                            //if (slotDuration == null)
                            //{
                            //    var message = string.Format("Slot duration {0} not found", slot_duration);
                            //    return message;
                            //}

                            //int slotId = slotDuration.sdid;




                            //string[] d = obj.time.Split(':');
                            //var slot_id = (new DLWFMSlot()).GetSlotDetail(slotId, Convert.ToInt32(d[0]));
                            //string slotid = obj.date?.ToString("yyyyMMdd") + slot_id.slotid;

                            ////--------------------------------------------------------
                            //DateTime appointment_date;
                            //int maste_slot_id;
                            //GetValue(slotid, out appointment_date, out maste_slot_id);

                            //Route_Issue ri = context.Route_Issues.Where(s => s.hpsm_ticketid == orderObj.hpsm_ticket_id).FirstOrDefault();
                            //var slot = (new DLWFMSlotConfirmation()).GetFreeSlot(slotid, ri.manager_id);

                            //string fe_type = context.User.Where(u => u.user_id == ri.user_id).FirstOrDefault().user_type; //"regular" : "fastlane";



                            //string jo_type = "";
                            //string service = "";

                            //var joType = (new DLWFMJotypeMaster()).GetJoType(SlotRequest.order_type); //New Installation

                            //if (joType == null)
                            //{
                            //    var message = string.Format("Order type not found");
                            //    return message;
                            //}
                            //jo_type = SlotRequest.order_type;


                            //service = SlotRequest.service_type;

                            //if (service.Contains(","))
                            //{
                            //    service = service.Split(',').First();
                            //}


                            //var AVFEList = new List<FEList>();

                            //var FEList = (new DLUserMaster()).GetFEList(SlotRequest.managerid).Where(f => f.user_type == fe_type).ToList();
                            //if (FEList.Count == 0)
                            //{
                            //    var message = string.Format("No FE map with contractor");
                            //    return message;

                            //}
                            ////case 6
                            //var _isUserjotype = FEList.Where(j => j.user_Jo_type == jo_type).ToList();
                            //if (FEList.Count == 0)
                            //{
                            //    var message = string.Format("FE user found but not map with jo type");
                            //    return message;

                            //}
                            ////case 7
                            //var _isUserService = _isUserjotype.Where(j => j.user_service == service).ToList();
                            //if (_isUserService.Count == 0)
                            //{
                            //    var message = string.Format("FE user found but not map with service");
                            //    return message;
                            //}
                            //AVFEList = _isUserService;


                            //#region Check roster

                            ////string strTimeSheetError = "";
                            //DateTime dtCurDate = DateTime.Now;//
                            //int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                            //AVFEList.ForEach(r =>
                            //{
                            //    string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                            //    var isTimeSheet = DLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, appointment_date, intDayOfWeek, strTime);

                            //    if (isTimeSheet != null)
                            //    {
                            //        r.isRosterAvailable = true;
                            //        r._start_time = isTimeSheet._start_time;
                            //        r._end_time = isTimeSheet._end_time;
                            //    }
                            //    else
                            //    {
                            //        r.isRosterAvailable = false;
                            //    }
                            //});

                            //if (AVFEList.Count > 0 && !AVFEList.Any(a => a.isRosterAvailable == true))
                            //{

                            //    var message = AVFEList.Count + " FE found but no roster available";
                            //    return message;
                            //}
                            //var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

                            //#endregion

                            DateTime appointment_date;
                            int maste_slot_id;
                            GetValue(obj.slotid, out appointment_date, out maste_slot_id);
                            // int totalslot = 10;// AvailableEF.Count - slot;

                            //if (totalslot > 0)
                            //{
                            DLWFMSlotConfirmation dd = new DLWFMSlotConfirmation();
                            dd.UpdateSlot(new SlotConfirmation() { slotid = obj.slotid, bookingid = orderObj.bookingid, appointment_date = appointment_date, master_slot_id = maste_slot_id, updated_date = DateTime.Now });
                            //--------------------------------------------------------//
                            var sr = (new DLWFMTicket().updateSlotRequestAppmntDate(Request.referenceid, appointment_date));
                            orderObj.resolution_close_id = obj.rca;

                            var isSentMail = Convert.ToBoolean(ConfigurationManager.AppSettings["isSentMail"]);
                            if (isSentMail)
                            {
                                var slotdetails = (new DLWFMSlot()).GetSlotDetailById(maste_slot_id);

                                TimeSpan span = TimeSpan.FromHours(slotdetails.from_time);
                                DateTime time = DateTime.Today + span;
                                String sp1 = time.ToString("hh:mm tt");

                                TimeSpan span2 = TimeSpan.FromHours(slotdetails.to_time);
                                DateTime time2 = DateTime.Today + span2;
                                String sp2 = time2.ToString("hh:mm tt");
                                var ap_date = appointment_date + " " + sp1 + " to " + sp2;


                                var template = (new DLWFMNotification()).GetTemplateDetail("reschedule");
                                if (template != null)
                                {
                                    var message = template.email.Replace("#date", ap_date);
                                    var isMailsent = Email.sendMail("", orderObj.email_id, template.subject, message);
                                    //send mail
                                    var id = (new DLWFMEmailsmsLog()).InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = orderObj.hpsmid, emailstatus = isMailsent == true ? 1 : 0, emaildeliverytime = DateTime.Now, emailremark = "", type = template.type });

                                    if (!string.IsNullOrEmpty(orderObj.customer_rmn))
                                    {
                                        string smsremark = "";
                                        int sentsms = SMS.SendSms(ref smsremark, orderObj.customer_rmn, message);
                                        (new DLWFMEmailsmsLog()).InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = orderObj.hpsmid, smsstatus = sentsms, smsdeliverytime = DateTime.Now, smsremark = smsremark, type = template.type });
                                    }

                                }
                            }

                            //}
                            //else
                            //{
                            //    var message = "Sorry ! Slot booked";
                            //    return message;
                            //}
                        }

                        repo.Update(orderObj);
                        new DLWFMTask().priority(orderObj.hpsmid);

                        Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.hpsm_ticketid == orderObj.hpsm_ticket_id).FirstOrDefault();
                        if (taskTracking != null)
                        {
                            taskTracking.status = objDetail.status;
                            taskTracking.sub_status = objDetail.sub_status;
                            taskTracking.modified_on = DateTime.Now;
                            if (!string.IsNullOrEmpty(obj.remarks))
                                taskTracking.remarks = obj.remarks;
                            context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        return "success";
                    }
                    else
                    {
                        return "fail";
                    }
                }
            }
            catch { throw; }

        }

        public int Checkin(checkin obj)
        {
            try
            {
                var orderObj = repo.Get(b => b.hpsmid == obj.job_id);
                if (orderObj != null)
                {
                    orderObj.latitude = obj.latitude;
                    orderObj.longitude = obj.longitude;
                    orderObj.status = "Checked In";
                    repo.Update(orderObj);
                    return orderObj.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch { throw; }

        }
        public int UpdateJobOrderStage(string job_id, int stage, string sub_status, string remark)
        {
            try
            {
                var orderObj = repo.Get(b => b.hpsmid == job_id);
                if (orderObj != null)
                {

                    //update if db stage is less then the current stage
                    if (Convert.ToInt32(orderObj.stage) < stage)
                    {
                        orderObj.stage = stage;
                    }
                    if (sub_status != null) { orderObj.status = sub_status; }
                    if (remark != null) { orderObj.remarks = remark; }
                    repo.Update(orderObj);
                    return orderObj.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch
            {
                throw;
            }

        }
        public int DeleteAttachementUpdateStage(string job_id, int stage)
        {
            try
            {
                var orderObj = repo.Get(b => b.hpsmid == job_id);
                if (orderObj != null)
                {

                    //update when delete attachment 
                    orderObj.stage = stage;
                    repo.Update(orderObj);
                    return orderObj.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch
            {
                throw;
            }

        }
        public int updateCpeDetailFromXml(string job_id, string refSerial, string itemCode, string uom, string wh)
        {

            try
            {
                var orderObj = repo.Get(b => b.hpsmid == job_id);
                if (orderObj != null)
                {
                    orderObj.cpe_item_code = itemCode;
                    orderObj.cpe_ref_serial = refSerial;
                    orderObj.cpe_uom = uom;
                    orderObj.cpe_wh = wh;
                    repo.Update(orderObj);
                    return orderObj.hpsm_ticket_id;
                }
                else
                {
                    return Convert.ToInt32(-1);
                }
            }
            catch { throw; }

        }


        public dynamic getTriggerActivateDetails(string jobId, out string facility)
        {

            facility = string.Empty;
            try
            {
                //Models.WFM.TriggerActivateDetail obj = new Models.WFM.TriggerActivateDetail();
                var entity = repo.GetAll(b => b.hpsmid == jobId).FirstOrDefault();
                string retrriger = "N";

                if(entity.isresolve!="0")
                {
                    retrriger = "Y";
                }
                if (entity != null)
                {
                    facility = entity.facility;
                    if (facility == "CATV" || facility == "CATV_EXTN")
                    {

                        dynamic CATV =
                        new
                        {
                            orderId = entity.parent_hpsmid,
                            casSerialNo = entity.cpe_serialno,
                            //stbSerialNo = "1111"
                            stbSerialNo = entity.device_serial_number1,
                            atomicOrderId = entity.atomic_id
                        };

                        return CATV;

                    }
                    else if (facility == "GPON")
                    {

                        dynamic Nornal =
                        new
                        {
                            //orderid = "oo870026",
                            //portnumber = "123",
                            //macid = "12:45:af:34:ed:92",
                            //serialno = "ztegc4a84955",
                            //cpemodel = "dpc2100r3",
                            //cpemake = "d2_air1000_10mb_new",
                            //stbserialno = "9876",

                            orderId = entity.parent_hpsmid,
                            atomicOrderId = entity.atomic_id,
                            napName = entity.node,
                            napPort = entity.nap_port,
                            macId = entity.cpe_ref_serial,
                            cpeItemCode = entity.cpe_item_code,
                            //portNumber = entity.cpe_portno,
                            serialNo = entity.cpe_serialno,
                            retrigger = retrriger,
                            //cpeModel = entity.cpe_model,
                            //cpeMake = entity.cpe_brand,

                            //stbSerialNo = "9876",

                        };
                        return Nornal;
                    }

                    else if (facility == "DOCSIS")
                    {

                        dynamic Nornal =
                        new
                        {
                            orderId = entity.parent_hpsmid,
                            atomicOrderId = entity.atomic_id,
                            //portNumber = entity.cpe_portno,
                            macId = entity.cpe_ref_serial,
                            cpeItemCode = entity.cpe_item_code,
                            serialNo = entity.cpe_serialno,
                            retrigger = retrriger,
                            //cpeModel = entity.cpe_model,
                            //cpeMake = entity.cpe_brand,

                        };
                        return Nornal;
                    }

                    else if (facility == "GFAST")
                    {

                        dynamic Nornal =
                        new
                        {
                            orderId = entity.parent_hpsmid,
                            atomicOrderId = entity.atomic_id,
                            napName = entity.node,
                            napPort = entity.nap_port,
                            macId = entity.cpe_ref_serial,
                            cpeItemCode = entity.cpe_item_code,
                            serialNo = entity.cpe_serialno,
                            retrigger = retrriger,
                            //portNumber = entity.cpe_portno,
                            //cpeModel = entity.cpe_model,
                            //cpeMake = entity.cpe_brand,
                        };

                        return Nornal;

                    }
                    else if (facility == "IPTV" || facility == "IPTV_EXTN")
                    {
                        dynamic Nornal =
                        new
                        {
                            orderId = entity.parent_hpsmid,
                            atomicOrderId = entity.atomic_id,
                            stbSerialNo = entity.cpe_serialno,
                            retrigger = retrriger

                        };

                        return Nornal;
                    }
                }

                //if (entity != null)
                //{
                //    obj.orderId = entity.hpsmid;
                //    obj.portNumber = entity.cpe_portno;
                //    obj.serialNo = entity.cpe_serialno;
                //    obj.cpeModel = entity.cpe_model;
                //    obj.cpeMake = entity.cpe_brand;
                //    obj.macId = entity.cpe_ref_serial;
                //    obj.stbSerialNo = "9876";
                //    return obj;
                //}
                //else
                //{
                //    return null;
                //}

                //return obj;
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public dynamic GetReTriggerRequest(string jobId)
        {
            try
            {
                var entity = repo.GetAll(b => b.hpsmid == jobId).FirstOrDefault();

                if (entity != null)
                {
                    dynamic Nornal =
                      new
                      {

                          //  newSerialNo="ZTEGC4A84955",
                          //  newMacId="12:45:AF:34:ED:92",
                          //  oldSerialNo="ZTEGC4A84955",
                          //  cpeItemCode="DDFERER",
                          // customerId="CC528040",
                          //associatedTicketId="1382090505"
                          newSerialNo = entity.cpe_serialno,
                          oldSerialNo = entity.current_cpesn,
                          orderId = entity.task_id,
                          retrigger="Y"

                      };
                    return Nornal;

                }


                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public dynamic CPEReplace(string jobId)
        {
            try
            {
                var entity = repo.GetAll(b => b.hpsmid == jobId).FirstOrDefault();
               
                if (entity != null)
                {
                      dynamic Nornal =
                        new
                        {

                            //  newSerialNo="ZTEGC4A84955",
                            //  newMacId="12:45:AF:34:ED:92",
                            //  oldSerialNo="ZTEGC4A84955",
                            //  cpeItemCode="DDFERER",
                            // customerId="CC528040",
                            //associatedTicketId="1382090505"
                            newSerialNo = entity.cpe_serialno,
                            newMacId = entity.cpe_mac_address,
                            oldSerialNo = entity.current_cpesn,
                            cpeItemCode =entity.cpe_item_code,
                            customerId = entity.customer_id,
                            associatedTicketId = entity.hpsmid,
                           //  napName = entity.node,
                            //napPort = entity.nap_port
                        
                        };
                        return Nornal;
                    
                }

              
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Route_Issue nEditTimeSheet(int issueId)
        {

            try
            {
                using (MainContext context = new MainContext())
                {
                    Route_Issue Route_Issue = context.Route_Issues.Where(s => s.issue_id == issueId).FirstOrDefault();
                    return Route_Issue;

                }
            }
            catch { throw; }

        }



    }

    public class DLHPSMTICKETATTACHMENTSView : Repository<Models.WFM.vw_hpsm_ticket_attachments>
    {
        public List<vw_hpsm_ticket_attachments> getAttachmentDetailsbyJobId(string job_id, string upload_type, string screen)
        {
            List<vw_hpsm_ticket_attachments> responce = new List<vw_hpsm_ticket_attachments>();
            try
            {

                responce = repo.GetAll(b => b.job_id == job_id && b.upload_type == upload_type && b.screen == screen).ToList();
                return responce;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<vw_hpsm_ticket_attachments> getAttachmentDetailsbyJobId(string job_id)
        {
            List<vw_hpsm_ticket_attachments> responce = new List<vw_hpsm_ticket_attachments>();
            try
            {

                responce = repo.GetAll(b => b.job_id == job_id.ToString()).ToList();
                return responce;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DLHPSMTICKETATTACHMENTS : Repository<Models.WFM.hpsm_ticket_attachments>
    {
        public List<hpsm_ticket_attachments> getAttachmentDetailsbyJobId(string job_id, string upload_type, string screen)
        {
            List<hpsm_ticket_attachments> responce = new List<hpsm_ticket_attachments>();
            try
            {

                responce = repo.GetAll(b => b.job_id == job_id && b.upload_type == upload_type && b.screen == screen).ToList();
                return responce;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public hpsm_ticket_attachments SaveTicketAttachment(hpsm_ticket_attachments objAttachment)
        {
            try
            {
                var resultItem = repo.Insert(objAttachment);
                return resultItem;
            }
            catch { throw; }
        }
        public hpsm_ticket_attachments getAttachmentDetailsbyId(int DocumentId)
        {
            hpsm_ticket_attachments responce = new hpsm_ticket_attachments();
            try
            {

                responce = repo.GetAll(b => b.id == DocumentId).FirstOrDefault();
                return responce;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteAttachmentById(int DocumentId)
        {
            try
            {
                var objSystmId = repo.Get(m => m.id == DocumentId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
    }





    public class DLWFMSlot : Repository<Slot>
    {
        public List<SlotResponse> GetSlot(int slotid, int rosterFromTime, int rosterToTime, DateTime appointmentDate, string referenceId, int Avfecount, List<FEList> fELists , int managerUserId, string managerName)
        {
            List<SlotResponse> responce = new List<SlotResponse>();
            try
            {
                List<Slot> slotList = new List<Slot>();

                string APDate = appointmentDate.ToString("yyyyMMdd");

                repo.GetAll(b => b.sdid == slotid).ToList().ForEach(f =>
                {
                    slotList.Add(new Slot()
                    {
                        from_time = Convert.ToInt32(Convert.ToString(f.from_time) + "00"),
                        to_time = Convert.ToInt32(Convert.ToString(f.to_time) + "00"),
                        sdid = f.sdid,
                        slotid = f.slotid
                    });
                });
                var entity = slotList.Where(b => b.from_time >= rosterFromTime && b.from_time < rosterToTime).ToList();
                //var entity = repo.GetAll(b => b.sdid == slotid && b.from_time + 00 >= rosterFromTime && b.from_time < rosterToTime).ToList();
                if (appointmentDate == DateTime.Now.Date)
                {
                    int currentHH = Convert.ToInt32(Convert.ToString(DateTime.Now.Hour) + "00");// ;
                    entity = entity.Where(t => t.from_time > currentHH).ToList();
                }
                entity.ForEach(t =>
                {
                    // int count = (Avfecount - (new DLWFMSlotConfirmation()).GetFreeSlot((APDate + t.slotid), managerUserId));

                    int appcount = 0;
                    var slotdetails = GetSlotDetailById(t.slotid);
                    //total booked slot for mamanger 10
                    var getSlots = (new DLWFMVMSConfirmation()).GetAllFreeSlot(managerUserId, appointmentDate);

                    if (getSlots.Count > 0)
                    {
                        //8-10   2
                        appcount = getSlots.Where(w => w.from_time == slotdetails.from_time).Count();

                        if (appcount <= 0)
                        {
                            appcount = getSlots.Where(w => w.to_time == slotdetails.to_time).Count();
                        }

                    }

                    //get actual FE count ( slot it, list of fe)
                    Avfecount = 0;
                    Avfecount = GetActualFECount(t.from_time,t.to_time,fELists);

                    int count = (Avfecount - appcount);

                    if (count > 0)
                    {
                        responce.Add
                        (
                         new SlotResponse()
                         {
                             slotid = APDate + t.slotid+"_"+ managerUserId,
                             temp_slotid= APDate + t.slotid,
                             //from_time = t.from_time,//Convert.ToInt32(Convert.ToString(t.from_time) + "00"),
                             //to_time =t.to_time ,//Convert.ToInt32(Convert.ToString(t.to_time) + "00"),
                             from_time = t.from_time.ToString().Length == 3 ? "0" + t.from_time : Convert.ToString(t.from_time),
                             to_time = t.to_time.ToString().Length == 3 ? "0" + t.to_time : Convert.ToString(t.to_time),
                             referenceId = referenceId,
                             appointment_date = appointmentDate.ToString("yyyy-MM-dd"),
                             dispatcher_name= managerName,
                         }
                        ); ;
                    }
                });

                return responce;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetActualFECount(int slotFromTime, int slotToTime, List<FEList> fELists)
        {
           
            var entity = fELists.Where(b => b._start_time <= slotFromTime && b._end_time >= slotToTime).ToList();
            return entity.Count;
        }
        public Slot GetSlotDetail(int slotid, int from_time)
        {
            Slot slot = new Slot();
            var res = repo.GetAll(b => b.sdid == slotid && b.from_time == from_time).FirstOrDefault();
            if (res != null)
            {
                slot = res;
            }
            return slot;
        }

        public Slot GetSlotDetailById(int slotid)
        {
            Slot slot = new Slot();
            var res = repo.GetAll(b => b.slotid == slotid).FirstOrDefault();
            if (res != null)
            {
                slot = res;
            }
            return slot;
        }
    }


    public class DLWFMSlotConfirmation : Repository<SlotConfirmation>
    {
        public int GetFreeSlot(string slotid, int managerUserId)
        {
            try
            {
                var entity = repo.GetAll(b => b.slotid == slotid && b.managerid == managerUserId && b.iscanceled == 0).ToList().Count;
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int InsertSlotConfirmation(SlotConfirmation obj)
        {
            try
            {
                repo.Insert(obj);
                return Convert.ToInt32(obj.bookingid);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public int UpdateSlotConfirmation(SlotConfirmation obj)
        {
            try
            {
                var entity = repo.Get(b => b.referenceid == obj.referenceid);
                if (entity != null)
                {
                    entity.slotid = obj.slotid;
                    entity.appointment_date = obj.appointment_date;
                    entity.managerid = obj.managerid;
                    entity.master_slot_id = obj.master_slot_id;
                    entity.updated_date = obj.updated_date;
                    repo.Update(entity);
                    return Convert.ToInt32(entity.bookingid);
                }
                else
                {
                    return Convert.ToInt32(-2);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public int UpdateSlot(SlotConfirmation obj)
        {
            try
            {
                var entity = repo.Get(b => b.bookingid == obj.bookingid);
                if (entity != null)
                {
                    entity.slotid = obj.slotid;
                    entity.appointment_date = obj.appointment_date;
                    entity.master_slot_id = obj.master_slot_id;
                    entity.updated_date = obj.updated_date;
                    repo.Update(entity);
                    return Convert.ToInt32(entity.bookingid);
                }
                else
                {
                    return Convert.ToInt32(-2);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public SlotConfirmation GetSlotConfirmation(int bookingId)
        {
            try
            {
                SlotConfirmation result = new SlotConfirmation();
                var entity = repo.GetAll(b => b.bookingid == bookingId && b.iscanceled == 0);
                if (entity != null)
                {
                    result = entity.FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public SlotConfirmation GetSlotConfirmationByRefId(string referenceid, int iscanceled)
        {
            try
            {
                SlotConfirmation result = new SlotConfirmation();
                var entity = repo.GetAll(b => b.referenceid == referenceid && b.iscanceled == iscanceled);
                if (entity != null)
                {
                    result = entity.FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public SlotConfirmation GetSlotConBySlotRefId(string slotid, string referenceid)
        {
            try
            {
                SlotConfirmation result = new SlotConfirmation();
                var entity = repo.GetAll(b => b.slotid == slotid && b.referenceid == referenceid);
                if (entity != null)
                {
                    result = entity.FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public AppointmentDetail GetAppointmentDetail(string referenceId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    AppointmentDetail objDetail = new AppointmentDetail();
                    AppointmentDetail result = new AppointmentDetail();
                    string query = string.Format(@"select wsc.bookingid,to_char(wsc.appointment_date,'YYYY-MM-dd') as appointment_date,ws.from_time,ws.to_time,wsr.order_type,wsr.service_type,wsc.referenceid from tbl_wfm_slot_request wsr  join 
                    tbl_wfm_slot_confirmation wsc on wsc.referenceid=wsr.referenceid
                    join tbl_wfm_slot ws on ws.slotid=wsc.master_slot_id
                    where wsr.referenceid= '{0}'", referenceId);
                    objDetail = context.Database.SqlQuery<AppointmentDetail>(query).FirstOrDefault();
                    result = objDetail;
                    if (objDetail != null)
                    {
                        result.bookingid = objDetail.bookingid;
                        result.order_type = objDetail.order_type;
                        result.service_type = objDetail.service_type;
                        result.from_time = Convert.ToInt32(Convert.ToString(objDetail.from_time) + "00");
                        result.to_time = Convert.ToInt32(Convert.ToString(objDetail.to_time) + "00");
                        result.referenceid = referenceId;
                        result.appointment_date = objDetail.appointment_date;
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public AppointmentDetail GetAppointmentDetails(string referenceId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    AppointmentDetail objDetail = new AppointmentDetail();
                    AppointmentDetail result = new AppointmentDetail();
                    string query = string.Format(@"select wsc.bookingid,to_char(wsc.appointment_date,'dd-MM-YYYY') as appointment_date,ws.from_time,ws.to_time,wsr.order_type,wsr.service_type,wsc.referenceid from tbl_wfm_slot_request wsr  join 
                    tbl_wfm_slot_confirmation wsc on wsc.referenceid=wsr.referenceid
                    join tbl_wfm_slot ws on ws.slotid=wsc.master_slot_id
                    where wsr.referenceid= '{0}'", referenceId);
                    objDetail = context.Database.SqlQuery<AppointmentDetail>(query).FirstOrDefault();
                    result = objDetail;
                    if (objDetail != null)
                    {
                        result.bookingid = objDetail.bookingid;
                        result.order_type = objDetail.order_type;
                        result.service_type = objDetail.service_type;
                        result.from_time = objDetail.from_time;
                        result.to_time = objDetail.to_time;
                        result.referenceid = referenceId;
                        result.appointment_date = objDetail.appointment_date;
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public int cancleAppointmentSlot(int bookingid)
        {
            try
            {

                SlotConfirmation result = new SlotConfirmation();
                var entity = repo.GetAll(b => b.bookingid == bookingid).FirstOrDefault();
                if (entity != null)
                {
                    entity.iscanceled = 1;
                    entity.updated_date = DateTime.Now;
                    repo.Update(entity);
                    return Convert.ToInt32(-1);
                }
                else
                {
                    return Convert.ToInt32(-2);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }




    }

    //public class DLWFMTask : Repository<Models.WFM.Task>
    //{



    //}

    public class DLWFMVMSConfirmation : Repository<VMSlotConfirmation>
    {
        public List<VMSlotConfirmation> GetAllFreeSlot(int managerUserId, DateTime appointment_date)
        {
            try
            {
                var entity = repo.GetAll(b => b.appointment_date == appointment_date && b.managerid == managerUserId && b.iscanceled == 0).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    public class DLWFMTask
    {
        public bool InsertHpsm_TicketMasterData(Models.WFM.Task obj)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    context.task.Add(obj);
                    return context.SaveChanges() > 0;
                }
            }
            catch { throw; }

        }

        public bool updateHpsm_TicketMasterData(Models.WFM.Task obj)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    return context.SaveChanges() > 0;
                }
            }
            catch { throw; }

        }

        public static bool isJobIdExist(string jobId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var entity = context.task.Where(b => b.hpsmid == jobId).ToList();
                    if (entity.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static int isJobIdExistCount(string jobId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var entity = context.task.Where(b => b.parent_hpsmid == jobId).ToList();
                    return entity.Count;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static List< Models.WFM.Payment_Details> GetPaymentDetailsByJobOrder(string job_id)
        {
            try
            {
                List<Models.WFM.Payment_Details> result = new List<Models.WFM.Payment_Details>();
                using (MainContext context = new MainContext())
                {
                    var entity = context.Payment_Details.Where(b => b.job_id == job_id);
                    if (entity != null)
                    {
                        result = entity.ToList();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Models.WFM.Task GetJobDetailByJobOrderId(string jobOrderId)
        {
            try
            {
                Models.WFM.Task result = new Models.WFM.Task();
                using (MainContext context = new MainContext())
                {
                    var entity = context.task.Where(b => b.hpsmid == jobOrderId);
                    if (entity != null)
                    {
                        result = entity.FirstOrDefault();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static Models.WFM.Task GetJobDetailByJobOrderIdAtomicId(string jobOrderId, string atomicOrderId)
        {
            try
            {
                Models.WFM.Task result = new Models.WFM.Task();
                using (MainContext context = new MainContext())
                {
                    result = context.task.Where(b => b.parent_hpsmid == jobOrderId && b.atomic_id == atomicOrderId).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static Models.WFM.Task GetTTJobDetailByJobOrderId(string jobOrderId)
        {
            try
            {
                Models.WFM.Task result = new Models.WFM.Task();
                using (MainContext context = new MainContext())
                {
                    result = context.task.Where(b => b.parent_hpsmid == jobOrderId ).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static bool isbookingIdExist(int bookingId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var entity = context.task.Where(b => b.bookingid == bookingId).ToList();
                    if (entity.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static int getrecordbyBookingId(int bookingId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var entity = context.task.Where(b => b.bookingid == bookingId).ToList();
                    return entity.Count;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static List<AssignedTaskDetail> GetFRTTaskDetails(int userId, string toDate, string fromDate, string status, string jobid)
        {
            try
            {
                List<AssignedTaskDetail> lstAssignedTaskDetail = null;

                using (MainContext context = new MainContext())
                {

                    var ruserId = new NpgsqlParameter("@P_ASSIGNEDID", userId);
                    var rFDate = new NpgsqlParameter("@P_FROMDATE", string.IsNullOrEmpty(fromDate) ? "" : fromDate);
                    var rTDate = new NpgsqlParameter("@P_TODATE", string.IsNullOrEmpty(toDate) ? "" : toDate);
                    var rStatus = new NpgsqlParameter("@P_STATUS", status);
                    var rJobId = new NpgsqlParameter("@p_jobid", jobid);
                    var parameters = new NpgsqlParameter[5] { ruserId, rFDate, rTDate, rStatus, rJobId };
                    lstAssignedTaskDetail = DbHelper.ExecutePostgresProcedure<AssignedTaskDetail>(context, "FN_GET_FRT_TASK", parameters);

                }
                return lstAssignedTaskDetail;
            }
            catch { throw; }
        }

        public static List<AssignedTaskDetailTT> GetTTFRTTaskDetails(int userId, string toDate, string fromDate, string status, string jobid)
        {
            try
            {
                List<AssignedTaskDetailTT> lstAssignedTaskDetail = null;

                using (MainContext context = new MainContext())
                {

                    var ruserId = new NpgsqlParameter("@P_ASSIGNEDID", userId);
                    var rFDate = new NpgsqlParameter("@P_FROMDATE", string.IsNullOrEmpty(fromDate) ? "" : fromDate);
                    var rTDate = new NpgsqlParameter("@P_TODATE", string.IsNullOrEmpty(toDate) ? "" : toDate);
                    var rStatus = new NpgsqlParameter("@P_STATUS", status);
                    var rJobId = new NpgsqlParameter("@p_jobid", jobid);
                    var parameters = new NpgsqlParameter[5] { ruserId, rFDate, rTDate, rStatus, rJobId };
                    lstAssignedTaskDetail = DbHelper.ExecutePostgresProcedure<AssignedTaskDetailTT>(context, "fn_get_frt_task_tt", parameters);

                }
                return lstAssignedTaskDetail;
            }
            catch { throw; }
        }


        public int priority(string jobId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    var entity = context.task.Where(b => b.hpsmid == jobId).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.priority = 1;
                        context.SaveChanges();
                        return 1;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool DeleteTaskTrackingByIssueId(int issue_id, int user_id = 0)
        {
            Task_Tracking objData = new Task_Tracking();

            try
            {
                using (MainContext context = new MainContext())
                {
                    Task_Tracking objTaskTracking = context.Task_Tracking.Where(s => s.issue_id == issue_id && s.frt_id == user_id).FirstOrDefault();
                    if (objTaskTracking != null)
                    {
                        objData = context.Task_Tracking.Find(objTaskTracking.task_tracking_id);

                        if (objData != null)
                        {
                            context.Entry(objData).State = System.Data.Entity.EntityState.Deleted;
                            return context.SaveChanges() > 0;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DLWFMRouteIssue
    {
        public bool SaveRouteIssue(Route_Issue objRouteIssue)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    context.Route_Issues.Add(objRouteIssue);
                    return context.SaveChanges() > 0;
                }
            }
            catch { throw; }

        }

        public Route_Issue GetRoute_Issue(int hpsmTicketId)
        {
            try
            {
                Models.WFM.Route_Issue result = new Route_Issue();
                using (MainContext context = new MainContext())
                {
                    var entity = context.Route_Issues.Where(b => b.hpsm_ticketid == hpsmTicketId);
                    if (entity != null)
                    {
                        result = entity.FirstOrDefault();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public Route_Issue GetHpsmidByRouteIssuesId(int issueid)
        {
            try
            {
                Models.WFM.Route_Issue result = new Route_Issue();
                using (MainContext context = new MainContext())
                {
                    var entity = context.Route_Issues.Where(b => b.issue_id == issueid);
                    if (entity != null)
                    {
                        result = entity.FirstOrDefault();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int cancleJobOrder(int hpsmTicketId)
        {
            try
            {
                using (var context = new MainContext())
                {

                    var entity = context.Route_Issues.Where(b => b.hpsm_ticketid == hpsmTicketId).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.status = "Cancelled";
                        context.SaveChanges();
                        return Convert.ToInt32(-1);
                    }
                    else
                    {
                        return Convert.ToInt32(-2);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }




        public static bool AssignRouteIssue(ViewManagerRouteIssueApprove objAssignIssue, out List<Vw_Hpsm_Ticket_Status> HpsmStatusList)
        {
            try
            {
                bool result = false;
                using (MainContext context = new MainContext())
                {
                    HpsmStatusList = new List<Vw_Hpsm_Ticket_Status>();
                    //edit mode
                    var ids = objAssignIssue.issuesId.Split(',');
                    DAUser objDAuser = new DAUser();
                    User user = objDAuser.getUserDetails(objAssignIssue.user_id);
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int issueId = Convert.ToInt32(ids[i]);

                        Route_Issue objRouteIssue = context.Route_Issues.Where(s => s.issue_id == issueId).FirstOrDefault();
                        string currentSubStatus = objRouteIssue.sub_status;
                        objRouteIssue.manager_remark = (objRouteIssue.manager_remark!=""? objRouteIssue.manager_remark + " || ":"") + user.user_name + ": ";
                        if (!string.IsNullOrEmpty(objAssignIssue.remarks))
                            objRouteIssue.manager_remark = objRouteIssue.manager_remark +objAssignIssue.remarks;

                        objRouteIssue.status = objAssignIssue.status;

                        if (objRouteIssue.manager_id != objAssignIssue.frtUserId)
                        {
                            objRouteIssue.sub_status = "Dispatched";
                        }


                        objRouteIssue.modified_on = DateTime.Now;
                        objRouteIssue.user_id = objAssignIssue.frtUserId;

                        context.Entry(objRouteIssue).State = System.Data.Entity.EntityState.Modified;

                        if (objRouteIssue.hpsm_ticketid != null && objRouteIssue.hpsm_ticketid != 0 && currentSubStatus != "Dispatched")
                        {
                            Vw_Hpsm_Ticket_Status objVw_Hpsm_Ticket_Status = GetHPSMTicketHistory(issueId, currentSubStatus);
                            if (objVw_Hpsm_Ticket_Status != null)
                            {
                                objVw_Hpsm_Ticket_Status.sub_status = objRouteIssue.sub_status;

                                HpsmStatusList.Add(objVw_Hpsm_Ticket_Status);
                            }
                        }

                        if (objRouteIssue.is_reopened == 1)
                        {

                            Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.issue_id == issueId).FirstOrDefault();
                            GetRouteTrackingObj(objRouteIssue, objAssignIssue, taskTracking);
                            context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;

                        }
                        else if (currentSubStatus != "Dispatched")
                        {
                            if (objRouteIssue.manager_id != objAssignIssue.frtUserId)
                            {
                                Task_Tracking objTaskTracking = GetRouteTrackingObj(objRouteIssue, objAssignIssue);
                                Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.issue_id == issueId).FirstOrDefault();
                         
                                if (taskTracking != null)
                                {

                                    GetRouteTrackingObj(objRouteIssue, objAssignIssue, taskTracking);
                                    taskTracking.assigned_date = DateTime.Now;
                                    taskTracking.status = "Assigned";
                                    objRouteIssue.status = "Assigned";
                                    context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;
                                    if(currentSubStatus== "Cancelled" || currentSubStatus=="UnInstallable")
                                    {
                                        Models.WFM.Task   task = context.task.Where(s => s.hpsm_ticket_id == objRouteIssue.hpsm_ticketid).FirstOrDefault();
                                        task.hpsmstatus = "Assigned";
                                        task.status = "Assigned";
                                        task.modified_on = DateTime.Now;
                                        context.Entry(task).State = System.Data.Entity.EntityState.Modified;
                                    }
                                }
                                else
                                {
                                    context.Task_Tracking.Add(objTaskTracking);
                                }

                            }
                        }
                        else if (objRouteIssue.status == "Re-Assigned")
                        {
                            Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.issue_id == issueId).FirstOrDefault();
                            GetRouteTrackingObj(objRouteIssue, objAssignIssue, taskTracking);
                            taskTracking.assigned_date = DateTime.Now;
                            taskTracking.status = "Assigned";
                            objRouteIssue.status = "Assigned";
                            context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;
                        }
                        else if (objRouteIssue.status == "UnAssigned")
                        {
                            objRouteIssue.sub_status = "";
                        }
                        result = context.SaveChanges() > 0;
                    }

                    return result;
                }

            }
            catch { throw; }
        }

        public static bool AssingContractorRouteIssue(ViewManagerRouteIssueApprove objAssignIssue)
        {
            try
            {
                bool result = false;
                using (MainContext context = new MainContext())
                {
                    DAUser objDAuser = new DAUser();
                    User user= objDAuser.getUserDetails(objAssignIssue.user_id);
                    var ids = objAssignIssue.issuesId.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int issueId = Convert.ToInt32(ids[i]);
                        Route_Issue objRouteIssue = context.Route_Issues.Where(s => s.issue_id == issueId).FirstOrDefault();
                        objRouteIssue.manager_remark = objRouteIssue.manager_remark+" || "+user.user_name + ": ";
                        if (!string.IsNullOrEmpty(objAssignIssue.remarks))
                            objRouteIssue.manager_remark = objRouteIssue.manager_remark + objAssignIssue.remarks;
                        objRouteIssue.modified_on = DateTime.Now;
                        objRouteIssue.user_id = objAssignIssue.frtUserId;
                        objRouteIssue.manager_id = objAssignIssue.frtUserId;
                        context.Entry(objRouteIssue).State = System.Data.Entity.EntityState.Modified;
                        result = context.SaveChanges() > 0;
                    }

                    return result;
                }

            }
            catch { throw; }
        }
        public static bool AssignRouteIssueTKT(ViewManagerRouteIssueApprove objAssignIssue, out List<Vw_Hpsm_Ticket_Status> HpsmStatusList)
        {
            try
            {
                bool result = false;
                using (MainContext context = new MainContext())
                {
                    HpsmStatusList = new List<Vw_Hpsm_Ticket_Status>();
                    //edit mode
                    var ids = objAssignIssue.issuesId.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int issueId = Convert.ToInt32(ids[i]);

                        Route_Issue objRouteIssue = context.Route_Issues.Where(s => s.issue_id == issueId).FirstOrDefault();
                        string currentSubStatus = objRouteIssue.sub_status;

                        if (!string.IsNullOrEmpty(objAssignIssue.remarks))
                            objRouteIssue.manager_remark = objAssignIssue.remarks;

                        objRouteIssue.status = objAssignIssue.status;

                        if (objRouteIssue.manager_id != objAssignIssue.frtUserId)
                        {
                            objRouteIssue.sub_status = "Dispatched";
                        }


                        objRouteIssue.modified_on = DateTime.Now;
                        objRouteIssue.user_id = objAssignIssue.frtUserId;

                        context.Entry(objRouteIssue).State = System.Data.Entity.EntityState.Modified;

                        //---------------------------------------------------------------------------------------------------------------------------------------------
                        if (objRouteIssue.hpsm_ticketid != null && objRouteIssue.hpsm_ticketid != 0 && currentSubStatus != "Dispatched")
                        {
                            Vw_Hpsm_Ticket_Status objVw_Hpsm_Ticket_Status = GetHPSMTicketHistory(issueId, currentSubStatus);
                            if (objVw_Hpsm_Ticket_Status != null)
                            {
                                objVw_Hpsm_Ticket_Status.sub_status = objRouteIssue.sub_status;

                                HpsmStatusList.Add(objVw_Hpsm_Ticket_Status);
                            }
                        }

                        if (objRouteIssue.is_reopened == 1)
                        {

                            Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.issue_id == issueId).FirstOrDefault();
                            GetRouteTrackingObj(objRouteIssue, objAssignIssue, taskTracking);
                            context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;

                        }
                        else if (currentSubStatus != "Dispatched")
                        {
                            if (objRouteIssue.manager_id != objAssignIssue.frtUserId)
                            {
                                Task_Tracking objTaskTracking = GetRouteTrackingObj(objRouteIssue, objAssignIssue);
                                context.Task_Tracking.Add(objTaskTracking);
                            }
                        }
                        else if (objRouteIssue.status == "Re-Assigned")
                        {
                            Task_Tracking taskTracking = context.Task_Tracking.Where(s => s.issue_id == issueId).FirstOrDefault();
                            GetRouteTrackingObj(objRouteIssue, objAssignIssue, taskTracking);
                            taskTracking.assigned_date = DateTime.Now;
                            taskTracking.status = "Assigned";
                            objRouteIssue.status = "Assigned";
                            context.Entry(taskTracking).State = System.Data.Entity.EntityState.Modified;
                        }
                        else if (objRouteIssue.status == "UnAssigned")
                        {
                            objRouteIssue.sub_status = "";
                        }
                        //------------------------------------------------------------------------------------------------------------------------------------------
                        result = context.SaveChanges() > 0;
                    }

                    return result;
                }

            }
            catch { throw; }
        }

        private static Vw_Hpsm_Ticket_Status GetHPSMTicketHistory(int issueId, string sub_status)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    Vw_Hpsm_Ticket_Status objVw_Hpsm_Ticket_Status = context.vw_Hpsm_Ticket_Status.Where(s => s.issue_id == issueId && s.sub_status == sub_status && s.userrole == 143).FirstOrDefault();
                    return objVw_Hpsm_Ticket_Status;
                }
            }
            catch (Exception op) { throw; }
        }

        private static void GetRouteTrackingObj(Route_Issue objRouteIssue, ViewManagerRouteIssueApprove objAssignIssue, Task_Tracking objTaskTracking)
        {
            objTaskTracking.issue_id = objRouteIssue.issue_id;
            objTaskTracking.frt_id = objAssignIssue.frtUserId;
            objTaskTracking.assigned_date = objAssignIssue.assignedDate;
            objTaskTracking.status = objAssignIssue.status;
            objTaskTracking.created_by = objAssignIssue.user_id;
            objTaskTracking.modified_by = objAssignIssue.user_id;
            objTaskTracking.checkin_radius = objAssignIssue.checkinRadius;
            objTaskTracking.modified_on = DateTime.Now;
            objTaskTracking.sub_status = objRouteIssue.sub_status;
            objTaskTracking.hpsm_ticketid = objRouteIssue.hpsm_ticketid;
            objTaskTracking.actual_lat = null;
            objTaskTracking.actual_lng = null;
            objTaskTracking.checkin_remarks = "";
            objTaskTracking.checkout_remarks = "";
            objTaskTracking.mobile_checkin_time = null;
            objTaskTracking.mobile_checkout_time = null;
            objTaskTracking.remarks = "";
            objTaskTracking.server_checkin_time = null;
            objTaskTracking.server_checkout_time = null;

        }

        private static Task_Tracking GetRouteTrackingObj(Route_Issue objRouteIssue, ViewManagerRouteIssueApprove objAssignIssue)
        {
            Task_Tracking objTaskTracking = new Task_Tracking();
            objTaskTracking.issue_id = objRouteIssue.issue_id;
            objTaskTracking.frt_id = objAssignIssue.frtUserId;
            // objTaskTracking.assigned_date = objAssignIssue.assignedDate;
            objTaskTracking.assigned_date = DateTime.Now;
            objTaskTracking.status = objAssignIssue.status;
            objTaskTracking.created_by = objAssignIssue.user_id;
            objTaskTracking.modified_by = objAssignIssue.user_id;
            objTaskTracking.checkin_radius = objAssignIssue.checkinRadius;
            objTaskTracking.modified_on = DateTime.Now;
            objTaskTracking.sub_status = objRouteIssue.sub_status;
            objTaskTracking.hpsm_ticketid = objRouteIssue.hpsm_ticketid;
            return objTaskTracking;
        }


    }

    public class DLWFMPaymentDetails
    {
        public bool SavePaymentDetails(Payment_Details objPaymentDetails)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    context.Payment_Details.Add(objPaymentDetails);
                    return context.SaveChanges() > 0;
                }
            }
            catch { throw; }

        }
    }
    public class DLHPSMTicket
    {
        public static bool UpdateRouteIssueStatus(ViewManagerRouteIssueApprove objSelf)
        {
            try
            {
                var result = false;
                using (MainContext context = new MainContext())
                {
                    //edit mode
                    var ids = (objSelf.issuesId ?? objSelf.issueId.ToString()).Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int issueId = Convert.ToInt32(ids[i]);
                        Route_Issue objRouteIssue = context.Route_Issues.Where(s => s.issue_id == issueId).FirstOrDefault();

                        if (!string.IsNullOrEmpty(objSelf.remarks))
                            objRouteIssue.manager_remark = objSelf.remarks;

                        objRouteIssue.status = objSelf.status;
                        objRouteIssue.modified_on = DateTime.Now;

                        context.Entry(objRouteIssue).State = System.Data.Entity.EntityState.Modified;
                        result = context.SaveChanges() > 0;
                    }
                }
                return result;
            }
            catch { throw; }
        }

        public static Models.WFM.Task Getjobdetails(string hpsmid)
        {
            using (MainContext context = new MainContext())
            {

                var obj = context.task.Where(p => p.hpsmid == hpsmid).OrderBy(p => p.modified_on).FirstOrDefault();
                return obj;

            }
        }

        public static int newEditTimeSheet(Models.WFM.Task obj)
        {
            return (new DLWFMJobOrderDetail()).newEditTimeSheet(obj);
        }
        public static nmsticket GetNMSTicket_Details(string ticketId)
        {
            using (MainContext context = new MainContext())
            {
                nmsticket objData = new nmsticket();
                var objTicketDetails = context.objNmsTicket.Where(p => p.ticketId.Equals(ticketId)).FirstOrDefault();

                if (objTicketDetails != null)
                {
                    objData = objTicketDetails;
                }
                return objData;

            }
        }
        public static Models.WFM.Task GetHPSMTicket_Detail(string ticketId)
        {
            using (MainContext context = new MainContext())
            {
                Models.WFM.Task objData = new Models.WFM.Task();
                var objTicketDetails = context.task.Where(p => p.hpsmid.Equals(ticketId)).FirstOrDefault();

                objData = objTicketDetails;

                //if (objTicketDetails != null)
                //{
                //    objData = objTicketDetails;
                //}
                return objData;

            }
        }


        public static Models.WFM.Task GetHPSMTicket_DetailById(int ticketId)
        {
            using (MainContext context = new MainContext())
            {
                Models.WFM.Task objData = new Models.WFM.Task();
                var objTicketDetails = context.task.Where(p => p.hpsm_ticket_id.Equals(ticketId)).FirstOrDefault();

                objData = objTicketDetails;

                //if (objTicketDetails != null)
                //{
                //    objData = objTicketDetails;
                //}
                return objData;

            }
        }

        public static List<VSF_TICKET_HISTORY> GetVSFTicketDetails(int ticketId)
        {
            using (MainContext context = new MainContext())
            {
                var objTicketDetails = context.Vsf_Ticket_History.Where(p => p.issue_id == ticketId).OrderBy(p => p.modified_on).ToList();
                return objTicketDetails;
            }

        }
        public static List<Models.WFM.ViewMaterialDetail> ViewMaterialDetail(string job_id)
        {
            List<ViewMaterialDetail> ViewMaterialDetails = new List<ViewMaterialDetail>();
            try
            {
                using (MainContext context = new MainContext())
                {
                    {
                        string query = string.Format(@"select * from vw_jo_order_material_detail where jobid = '{0}'", job_id);
                        ViewMaterialDetails = context.Database.SqlQuery<ViewMaterialDetail>(query).ToList();
                    }

                }
            }
            catch { throw; }
            return ViewMaterialDetails;
        }
        public static void SaveNotificationMessageInHistory(NOTIFICATION_ALERTS_HISTORY objNotification)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    context.Notification_Alerts_History.Add(objNotification);
                    context.SaveChanges();
                }
            }
            catch (Exception op) { throw; }
        }
        public static Models.WFM.Task GetjobdetailsbyId(int? hpsmticketid)
        {
            using (MainContext context = new MainContext())
            {

                var obj = context.task.Where(p => p.hpsm_ticket_id == hpsmticketid).OrderBy(p => p.modified_on).FirstOrDefault();
                return obj;

            }
        }

        public static List<VW_HPSM_Ticket_Master_History> GetHPSMTicket_Detail_Status_History(string ticketId)
        {
            List<VW_HPSM_Ticket_Master_History> objList = new List<VW_HPSM_Ticket_Master_History>();

            try
            {

                using (MainContext context = new MainContext())
                {

                    {

                        string query = string.Format(@"select * from vw_hpsm_ticket_status_history where hpsmid = '{0}' order by modified_on desc", ticketId);
                        objList = context.Database.SqlQuery<VW_HPSM_Ticket_Master_History>(query).ToList();
                    }



                }
            }
            catch { throw; }
            return objList;
        }


        public static List<FRTCapacity> GetTaskTrackingDetail(int ?userId)
        {

            List<FRTCapacity> UserDetails = null;
            using (MainContext context = new MainContext())
            {

                string query = string.Format(@"select * from vw_get_today_ticket_capacity where user_id = '{0}' ", userId);
                UserDetails = context.Database.SqlQuery<FRTCapacity>(query).ToList();

                return UserDetails;
            }
        }
        public static List<FRTCapacity> GetDispatcherCountDetail(int? userId, DateTime appointment_date)
        {

            List<FRTCapacity> UserDetails = null;
            using (MainContext context = new MainContext())
            {
                string a = appointment_date.ToString("yyyy-MM-dd");
                string query = string.Format(@"select * from vw_get_today_dispatcher_ticket where user_id = '{0}' and assigned_date='{1}' and  status <> 'Completed' ", userId, appointment_date.ToString("yyyy-MM-dd"));
                UserDetails = context.Database.SqlQuery<FRTCapacity>(query).ToList();
                return UserDetails;
            }
        }
        public static List<FRTCapacity> GetSupervisiorCountDetail(int? userId)
        {

            List<FRTCapacity> UserDetails = null;
            using (MainContext context = new MainContext())
            {
                string query = string.Format(@"select * from vw_get_supervisior_frt_ticket where user_id = '{0}' ", userId);
                UserDetails = context.Database.SqlQuery<FRTCapacity>(query).ToList();
                return UserDetails;
            }
        }
        public static getStatusDetail GetStatusDetailByJobOrderId(getDetailIn obj)
        {
            getStatusDetail getStatusDetail = new getStatusDetail();
            try
            {
                using (MainContext context = new MainContext())
                {
                    {
                        string query = string.Format(@"select hpsmid as jobid, hpsmstatus as status,status as sub_status,remarks from hpsm_ticket_master where hpsmid = '{0}'", obj.job_id);
                        getStatusDetail = context.Database.SqlQuery<getStatusDetail>(query).FirstOrDefault();
                    }

                }
            }
            catch { throw; }
            return getStatusDetail;
        }
        public static RCADetailByJobId getTTRCAbyJobId(string job_id)
        {
            RCADetailByJobId rcaDetailByJobId = new RCADetailByJobId();
            try
            {
                using (MainContext context = new MainContext())
                {
                    {
                        string query = string.Format(@"select hpsmid as job_Id, COALESCE(resolution_close_id,'') as rca,COALESCE(root_cause_id,'') as rc,COALESCE(remarks,'') as remarks  from hpsm_ticket_master where hpsmid = '{0}'", job_id);
                        rcaDetailByJobId = context.Database.SqlQuery<RCADetailByJobId>(query).FirstOrDefault();
                    }

                }
            }
            catch { throw; }
            return rcaDetailByJobId;
        }

        public int GetStepOrder()
        {
            try
            {

                using (MainContext context = new MainContext())
                {
                    string query = string.Format(@"select step_order from ticket_steps_master where UPPER(step_name)=UPPER('Activation')");
                    int stepOrder = context.Database.SqlQuery<int>(query).FirstOrDefault();
                    return stepOrder;
                }
            }
            catch { throw; }

        }


        public static int saveConnectedDeviceRequest(ConnectedDeviceRequest connectedDeviceRequest)
        {
            try
            {
                int result = 0;
                using (MainContext context = new MainContext())
                {
                    context.ConnectedDeviceRequest.Add(connectedDeviceRequest);
                    result = context.SaveChanges();
                }
                return result;
            }
            catch (Exception ex) { throw; }
        }

        public static List<ConnectedDevice> GetConnectedDevice(ConnectedDeviceDetail obj)
        {
            List<ConnectedDevice> connectedDevice = new List<ConnectedDevice>();
            try
            {
                using (MainContext context = new MainContext())
                {
                    {
                        connectedDevice = context.ConnectedDevice.Where(s => s.requestid == obj.requestid).ToList();
                    }

                }

            }
            catch { throw; }
            return connectedDevice;
        }

        public static List<WfmRca> GetRcadetail(rcaIn obj)
        {
            List<WfmRca> WfmRca = new List<WfmRca>();
            try
            {
                using (MainContext context = new MainContext())
                {
                    {
                        WfmRca = context.WfmRca.Where(s => s.status == obj.status).ToList();
                    }

                }

            }
            catch { throw; }
            return WfmRca;
        }

        public static List<rca> getrcakeyvalue(string status, int ticket_source)
        {
            List<rca> WfmRca = new List<rca>();
            try
            {
                using (MainContext context = new MainContext())
                {
                    {
                        context.WfmRca.Where(s => s.status.ToUpper() == status.ToUpper() && s.ticket_source== ticket_source).ToList().ForEach(
                             f =>
                             {
                                 WfmRca.Add(new rca()
                                 {
                                     key = f.rca,
                                     value = f.rca
                                 });
                             }
                         );


                    }

                }

            }
            catch { throw; }
            return WfmRca;
        }

        public static List<rca> getttrcakeyvalue(string status)
        {
            List<rca> WfmRca = new List<rca>();
            try
            {
                using (MainContext context = new MainContext())
                {
                    {
                        context.wfm_tt_rca.Where(s => s.rc_code.ToUpper() == status.ToUpper()).ToList().ForEach(
                             f =>
                             {
                                 WfmRca.Add(new rca()
                                 {
                                     key = f.rca_description,
                                     value = f.rca_description
                                 });
                             }
                         );


                    }

                }

            }
            catch { throw; }
            return WfmRca;
        }

        public static Models.WFM.ConnectedDeviceRequest ConnectedDeviceDetailByRequestId(string requestId)
        {
            try
            {
                Models.WFM.ConnectedDeviceRequest result = new Models.WFM.ConnectedDeviceRequest();
                using (MainContext context = new MainContext())
                {
                    var entity = context.ConnectedDeviceRequest.Where(b => b.requestid == requestId);
                    if (entity != null)
                    {
                        result = entity.FirstOrDefault();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static int updateConnectedDevice(Models.WFM.Root root)
        {
            try
            {
                int result = 0;
                List<ConnectedDevice> ConnectedDevice = new List<ConnectedDevice>();
                ConnectedDevice = root.data;
                using (MainContext context = new MainContext())
                {
                    ConnectedDeviceRequest ConnectedDeviceRequest = context.ConnectedDeviceRequest.Where(s => s.requestid == root.requestid).FirstOrDefault();
                    ConnectedDeviceRequest.status = root.status;
                    if (!string.IsNullOrEmpty(root.message))
                        ConnectedDeviceRequest.message = root.message;
                    context.Entry(ConnectedDeviceRequest).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                    if (ConnectedDevice.Count > 0)
                    {
                        for (int i = 0; i < ConnectedDevice.Count; i++)
                        {
                            ConnectedDevice[i].requestid = root.requestid;
                            context.ConnectedDevice.Add(ConnectedDevice[i]);
                            result = context.SaveChanges();
                        }
                    }



                }
                return result;
            }
            catch (Exception ex) { throw; }
        }

    }
    public class DLIssues
    {
        public static Issue_Resolution_Type_Master GetIssueResolutonTypeByResolutionCode(string ResolutionCode)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    return context.issue_resolution_type_master.Where(x => x.issue_resolution_code == ResolutionCode).FirstOrDefault();
                }
            }
            catch { throw; }
        }
        public static int IssueCircleCount(string IssueIds, out int circle_id)
        {
            var inNumArray = IssueIds.Split(',').Select(Int32.Parse).ToList();

            using (MainContext context = new MainContext())
            {
                try
                {
                    List<Route_Issue> objUserDetail = new List<Route_Issue>();
                    var objIssueList = (from s in context.Route_Issues // outer sequence
                                        join st in context.task on s.hpsm_ticketid equals st.hpsm_ticket_id
                                        where inNumArray.Contains(s.issue_id)
                                        select st).ToList();
                    var intCount = objIssueList.GroupBy(c => c.circle_name).Count();

                    circle_id = 0;
                    //commenting due to table missing 
                    //if (intCount != 0)
                    //{
                    //    var objCircle = DADashBoard.GetCircleList().Where(x => x.circle_description == objIssueList[0].circle_name).FirstOrDefault();

                    //    if (objCircle != null)
                    //    {
                    //        circle_id = objCircle.circle_id;
                    //    }
                    //}
                    return intCount;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static List<VW_ROUTE_ASSIGNED_ISSUES> GetAssignIssueRouteDetails(ViewAssignJobOrderFilter viewIssueRouteFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_ROUTE_ASSIGNED_ISSUES> routeAssignDetails = null;
                using (MainContext context = new MainContext())
                {



                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.issueType) ? "" : viewIssueRouteFilter.issueType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());

                    var parameters = new NpgsqlParameter[13] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus };
                    routeAssignDetails = DbHelper.ExecutePostgresProcedure<VW_ROUTE_ASSIGNED_ISSUES>(context, "FN_GET_ASSIG_ROUTE_ISSUE_DET", parameters, out totalRecords);

                    return routeAssignDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }
    public static class DADashBoard
    {

        public static List<Circle_Master> GetCircleList()
        {
            List<Circle_Master> lstCircles = new List<Circle_Master>();
            try
            {
                using (MainContext context = new MainContext())
                {
                    lstCircles = context.circle_master.OrderBy(s => s.circle_code).ToList();
                    return lstCircles;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }


    public class DLWFMAddMaterilaMaster : Repository<AdditionalMaterialMaster>
    {
        public List<AdditionalMaterialMaster> GetAdditionalMaterialMaster(string ticket_type = null)
        {
            try
            {
                var result = repo.GetAll().Where(q => q.ticket_type == ticket_type && q.is_active);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public AdditionalMaterialMaster GetAdditionalMaterialMasterById(int pid)
        {
            try
            {
                return repo.GetById(pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DLWFMAddMaterial : Repository<AdditionalMaterial>
    {
        public int SaveAdditionalMaterial(string jobid, List<AdditionalMaterial> obj)
        {
            int res = 0;
            try
            {
                var lst = repo.GetAll(x => x.jobid == jobid);
                if (lst != null)
                {
                    var rs = repo.DeleteRange(lst.ToList());
                    if (rs > 0)
                    {
                        repo.Insert(obj);
                        res = 1;
                        return res;
                    }
                }
                return res;
            }
            catch { throw; }
        }

        public List<AdditionalMaterial> GetAdditionalMaterial(string jobid)
        {
            try
            {
                var result = repo.GetAll(x => x.jobid == jobid);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }


    public class DLwfm_jobstatus : Repository<tbl_wfm_jobstatus>
    {


        public List<job_status_type_rca> GetJobOrderstatus(string action, int ticket_source)
        {
            try
            {
                List<job_status_type_rca> ob = new List<job_status_type_rca>();
                var result = repo.GetAll(x => x.action == action);
                result.ToList().ForEach(f =>
                {
                    //ob.Add(new job_status_type() 
                    //{
                    //    key = f.status, value = f.sub_status 
                    //});

                    ob.Add(new job_status_type_rca()
                    {
                        key = f.status,
                        value = f.sub_status,
                        rca = DLHPSMTicket.getrcakeyvalue(f.status, ticket_source)
                    });
                });



                return ob.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }


    public class DLWFMTTRC : Repository<wfm_tt_rc>
    {
        public List<job_status_type_rca> GetTTRcRca()
        {
            try
            {
                List<job_status_type_rca> ob = new List<job_status_type_rca>();
                var result = repo.GetAll();
                result.ToList().ForEach(f =>
                {
                    ob.Add(new job_status_type_rca()
                    {
                        key = f.rc_code,
                        value = f.rc_description,
                        rca = DLHPSMTicket.getttrcakeyvalue(f.rc_code)
                    });
                });

                return ob.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }


    public class DLUserMaster : Repository<User_Master>
    {
        public List<FEList> GetFEList(int ManagerUserId)
        {
            //try
            //{
            //    List<FEList> FEList = new List<FEList>();
            //    repo.GetAll(x => x.manager_id == ManagerUserId).ToList()
            //        .ForEach(f =>
            //        {
            //            FEList.Add(new FEList()
            //            {
            //                UserId = f.user_id,
            //                ManagerUserId = f.manager_id
            //            });
            //        });


            //    return FEList.ToList();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            try
            {
                List<FEList> FEList = new List<FEList>();
                using (MainContext context = new MainContext())
                {
                    var rUserId = new NpgsqlParameter("@USERID", ManagerUserId);
                    var parameters = new NpgsqlParameter[1] { rUserId };
                    FEList = DbHelper.ExecutePostgresProcedure<FEList>(context, "fn_wfm_get_fe_by_managerid", parameters);
                    return FEList;
                }
            }
            catch
            {
                throw;
            }
        }
        public List<FEList> GetFEListbylocation(int ManagerUserId, double lat, double lng, string role)
        {
       
            try
            {
                List<FEList> FEList = new List<FEList>();
                using (MainContext context = new MainContext())
                {
             //       var rUserId = new NpgsqlParameter("@USERID", ManagerUserId);
              //      var rlat = new NpgsqlParameter("@lat", lat);
               //     var rlng = new NpgsqlParameter("@lng", lng);
                 //   var rrole = new NpgsqlParameter("@role", role);
                 ////   var parameters = new NpgsqlParameter[4] { rUserId, rlat , rlng, rrole };
                //    FEList = DbHelper.ExecutePostgresProcedure<FEList>(context, "fn_wfm_get_fe_by_managerid_and_location", parameters);

                    string query = string.Format(@"SELECT pb.block_name, um.user_id as userid,
                um.manager_id as manageruserid ,um.user_type,rm.role_name as role_name 
                from tbl_block_boundary pb
                left join user_permission_area upa on pb.id = upa.block_id
                left join user_master um on um.user_id = upa.user_id
                left join role_master rm on rm.role_id = um.role_id
                left join user_manager_mapping umm on umm.user_id=um.user_id
                where
                ST_Within(St_Geomfromtext('POINT({0} {1})', 4326), sp_geometry) and um.is_active=true and (rm.role_name = '{2}' or rm.role_name is null) and umm.manager_id={3}", lng, lat, role,ManagerUserId);
                    FEList = context.Database.SqlQuery<FEList>(query).ToList();
                    return FEList;
                }
            }
            catch
            {
                throw;
            }
        }
        public string GetUserNameById(int ManagerUserId)
        {
            try
            {
                string username = "";
                List<FEList> FEList = new List<FEList>();
                var res = repo.GetAll(x => x.user_id == ManagerUserId);
                if (res != null)
                {
                    username = res.FirstOrDefault().name;
                }
                return username;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public  User_Master GetUserById(int UserId)
        {
            User_Master usermaster = new User_Master();
            try
            {
                using (MainContext context = new MainContext())
                {
                    usermaster = context.user_Masters.Where(x => x.user_id == UserId).SingleOrDefault();
                    return usermaster;
                }
            }
            catch { throw; }
        }

        public UserDetails GetUserRoleDetailsById(int userId)
        {
            UserDetails userDetails = new UserDetails();
            try
            {
                using (MainContext context = new MainContext())
                {
                    string query = string.Format(@"select * from vw_get_user_role_details where user_id = '{0}' ", userId);
                    userDetails = context.Database.SqlQuery<UserDetails>(query).FirstOrDefault();
                    return userDetails;
                }
            }
            catch { throw; }
        }

        public UserServiceDetail GetServiceDetailByUserId(int userid)
        {
            try
            {
                UserServiceDetail UserList = new UserServiceDetail();
                using (MainContext context = new MainContext())
                {
                    var rUserId = new NpgsqlParameter("@USERID", userid);
                    var parameters = new NpgsqlParameter[1] { rUserId };
                    var result = DbHelper.ExecutePostgresProcedure<UserServiceDetail>(context, "fn_wfm_get_service_by_userid", parameters);
                    if (result.Count > 0)
                    {
                        UserList = new UserServiceDetail()
                        {
                            service = string.Join(", ", result.GroupBy(g => g.service).Distinct().Select(s => s.Key).ToArray()),
                            transection = string.Join(", ", result.GroupBy(g => g.transection).Distinct().Select(s => s.Key).ToArray()),
                            jo_category_name = string.Join(", ", result.GroupBy(g => g.jo_category_name).Distinct().Select(s => s.Key).ToArray()),
                            user_type = string.Join(", ", result.GroupBy(g => g.user_type).Distinct().Select(s => s.Key).ToArray())
                        };

                    }
                    return UserList;
                }
            }
            catch
            {
                throw;
            }
        }


        public List<UserServiceDetail> GetFEDetailByUserId(int userid)
        {
            try
            {
                List<UserServiceDetail> UserList = new List<UserServiceDetail>();
                using (MainContext context = new MainContext())
                {
                    var rUserId = new NpgsqlParameter("@USERID", userid);
                    var parameters = new NpgsqlParameter[1] { rUserId };
                    UserList = DbHelper.ExecutePostgresProcedure<UserServiceDetail>(context, "fn_wfm_get_service_by_userid", parameters);
                    return UserList;
                }
            }
            catch
            {
                throw;
            }
        }
    }
    public class DLJoCatRoleMapping : Repository<JoCategoryRoleMapping>
    {
        public JoCategoryRoleMapping GetRoleNameByJoCategory(string jo_category)
        {
            try
            {
                JoCategoryRoleMapping result = new JoCategoryRoleMapping();
                var entity = repo.GetAll(b => b.jo_category == jo_category);
                if (entity != null)
                {
                    result = entity.FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JoCategoryRoleMapping GetRoleNameByJoCategory(string jo_category, string joType)
        {
            try
            {
                JoCategoryRoleMapping result = new JoCategoryRoleMapping();
                var entity = repo.GetAll(b => b.jo_category_code.ToUpper() == jo_category.ToUpper() && b.order_type_code.ToUpper() == joType.ToUpper());
                if (entity != null)
                {
                    result = entity.FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DLWFMOrderType : Repository<Issue_Type_Master>
    {
        public Issue_Type_Master GetOrderType(string orderType)
        {
            try
            {
                var entity = repo.Get(b => b.name == orderType);
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    public class DLWFMJotypeMaster : Repository<wfm_jo_type_master>
    {
        public wfm_jo_type_master GetJoType(string jo_name)
        {
            try
            {
                var entity = repo.Get(b => b.jo_code.ToUpper() == jo_name.ToUpper());
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }



    public class DLWFMServiceFacilityMaster : Repository<wfm_service_facility_master>
    {
        public wfm_service_facility_master GetServiceFacility(string facility_name)
        {
            try
            {
                var entity = repo.Get(b => b.service_facility_code.ToUpper() == facility_name.ToUpper());
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<wfm_service_facility_master> GetMainServiceFacility()
        {
            try
            {
                List<wfm_service_facility_master> obj = null;
                using (MainContext context = new MainContext())
                {
                     obj = context.wfm_service_facility_master.Where(s => s.slot_duration != 0).ToList();
               
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    public class DLWFMSlotDuration : Repository<tbl_wfm_slot_duration>
    {
        public tbl_wfm_slot_duration GetSlotDurationDetails(int slot_duration)
        {
            try
            {
                var entity = repo.Get(b => b.time_duration == slot_duration);
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DLWFMNotification : Repository<wfm_notification_template>
    {
        public wfm_notification_template GetTemplateDetail(string type)
        {
            try
            {
                var entity = repo.Get(b => b.type == type);
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DLWFMEmailsmsLog : Repository<wfm_email_sms_log>
    {
        public wfm_email_sms_log GetbyJobOrderId(string joborderid, string type)
        {
            try
            {
                var entity = repo.Get(b => b.joborderid == joborderid && b.type == type);
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int InsertEmailSmsLog(wfm_email_sms_log obj)
        {
            try
            {
                var entity = repo.Get(b => b.joborderid == obj.joborderid);
                if (entity != null)
                {
                    entity.smsstatus = obj.smsstatus;
                    entity.smsdeliverytime = obj.smsdeliverytime;
                    entity.smsremark = obj.smsremark;
                    repo.Update(entity);
                    return Convert.ToInt32(-1);
                }
                else
                {
                    repo.Insert(obj);
                    return Convert.ToInt32(obj.id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }


}
