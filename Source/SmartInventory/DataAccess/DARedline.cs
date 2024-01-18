using System;
using System.Collections.Generic;
using Models;
using Models.Admin;
using DataAccess.DBHelpers;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DARedline : Repository<RedlineMaster>
    {

        public List<Redline> GetRedLine(RedlineFilter objRedLineFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Redline>("fn_redline_get_details", new
                {
                    p_searchby = objRedLineFilter.searchByText,
                    p_searchtext = objRedLineFilter.searchText,
                    p_pageno = objRedLineFilter.currentPage,
                    p_pagerecord = objRedLineFilter.pageSize,
                    p_sortcolname = objRedLineFilter.sort,
                    p_sorttype = objRedLineFilter.sortdir,
                    p_searchfrom = objRedLineFilter.fromDate,
                    p_searchto = objRedLineFilter.toDate,
                    p_task_system_id = objRedLineFilter.objRedline.task_system_id,
                    p_task_name = objRedLineFilter.objRedline.task_name,
                    p_assigned_to = objRedLineFilter.objRedline.assigned_to,
                    p_assigned_by = objRedLineFilter.objRedline.assigned_by,
                    p_task_status = objRedLineFilter.objRedline.task_status,
                    p_userid = objRedLineFilter.userId,
                    p_status_id = objRedLineFilter.objRedline.status_id,

                }, true).ToList();
                return lst;

            }
            catch (Exception e) { throw; }
        }

        public List<Dictionary<string, string>> Get_RedlineEntity_History(RedlineFilter objRedlineFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_redline_get_entity_status_history", new
                {
                    p_pageno = objRedlineFilter.currentPage,
                    p_pagerecord = objRedlineFilter.pageSize,
                    p_sortcolname = objRedlineFilter.sort,
                    p_sorttype = objRedlineFilter.sortdir,
                    p_userid = objRedlineFilter.userId,
                    p_task_id = objRedlineFilter.task_id,
                    p_status = objRedlineFilter.objRedline.status,
                    p_status_updated_on = objRedlineFilter.objRedline.created_on,
                    p_status_updated_by = objRedlineFilter.objRedline.user_name,
                    p_remarks = objRedlineFilter.objRedline.remarks,


                }, true);
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public RedlineMaster GetRedlineById(int task_system_id)
        {
            try
            {
                return repo.Get(u => u.system_id == task_system_id);
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public RedlineMaster SaveRedline(RedlineMaster objRedlineMaster, int userId)
        {
            try
            {
                var objRedline = repo.Get(u => u.system_id == objRedlineMaster.system_id);
                if (objRedline != null)
                {
                    objRedline.task_name = objRedlineMaster.task_name;
                    objRedline.task_type = objRedlineMaster.task_type;
                    //objRedline.task_remarks = objRedlineMaster.task_remarks;
                    objRedline.task_action = objRedlineMaster.task_action;
                    objRedline.status_id = objRedlineMaster.status_id;
                    objRedline.modified_by = userId;
                    objRedline.modified_on = DateTimeHelper.Now;
                    objRedline.action = "U";
                    var response = repo.Update(objRedline);
                    if (response != null)
                    {
                        response.pageMsg = new PageMessage();
                        response.pageMsg.message = null;
                    }

                    foreach (string assignedUsersID in objRedlineMaster.lstAssignedUsers)
                    {
                        RedlineAssignedUsers objAssignedUsers = DARedlineAssignedUsers.Instance.GetAssignedUsersById(objRedlineMaster.system_id, Convert.ToInt32(assignedUsersID));
                        //objAssignedUsers.task_id = objRedlineMaster.system_id;
                        objAssignedUsers.status = objRedlineMaster.status_id;
                        objAssignedUsers.remarks = objRedlineMaster.task_remarks;
                        //objAssignedUsers.user_id = Convert.ToInt32(assignedUsersID);
                        DARedlineAssignedUsers.Instance.UpdateUserDetails(objAssignedUsers);
                    }
                    foreach (string assignedUsersID in objRedlineMaster.lstAssignedUsers)
                    {
                        RedlineStatusHistory objRedlineStatusHistory = new RedlineStatusHistory();
                        objRedlineStatusHistory.task_id = objRedlineMaster.system_id;
                        objRedlineStatusHistory.status = objRedlineMaster.status_id;
                        objRedlineStatusHistory.remarks = objRedlineMaster.task_remarks;
                        objRedlineStatusHistory.task_name = objRedlineMaster.task_name;
                        objRedlineStatusHistory.task_type = objRedlineMaster.task_type;
                        objRedlineStatusHistory.assigned_by = userId;
                        objRedlineStatusHistory.assigned_to = Convert.ToInt32(assignedUsersID);
                        objRedlineStatusHistory.created_on = DateTimeHelper.Now;
                        objRedlineStatusHistory.created_by = userId;

                        DARedlineStatusHistory.Instance.InsertHistoryDetails(objRedlineStatusHistory);
                    }

                    return response;
                }
                else
                {
                    objRedlineMaster.action = "I";
                    objRedlineMaster.created_on = DateTimeHelper.Now;
                    objRedlineMaster.created_by = userId;

                    objRedlineMaster = repo.Insert(objRedlineMaster);
                    foreach (string assignedUsersID in objRedlineMaster.lstAssignedUsers)
                    {
                        RedlineAssignedUsers objAssignedUsers = new RedlineAssignedUsers();
                        objAssignedUsers.task_id = objRedlineMaster.system_id;
                        objAssignedUsers.status = objRedlineMaster.status_id;
                        objAssignedUsers.remarks = objRedlineMaster.task_remarks;
                        objAssignedUsers.user_id = Convert.ToInt32(assignedUsersID);
                        DARedlineAssignedUsers.Instance.InsertUserDetails(objAssignedUsers);
                    }
                    foreach (string assignedUsersID in objRedlineMaster.lstAssignedUsers)
                    {
                        RedlineStatusHistory objRedlineStatusHistory = new RedlineStatusHistory();
                        objRedlineStatusHistory.task_id = objRedlineMaster.system_id;
                        objRedlineStatusHistory.task_name = objRedlineMaster.task_name;
                        objRedlineStatusHistory.task_type = objRedlineMaster.task_type;
                        objRedlineStatusHistory.status = objRedlineMaster.status_id;
                        objRedlineStatusHistory.remarks = objRedlineMaster.task_remarks;
                        objRedlineStatusHistory.assigned_by = userId;
                        objRedlineStatusHistory.assigned_to = Convert.ToInt32(assignedUsersID);
                        objRedlineStatusHistory.created_on = DateTimeHelper.Now;
                        objRedlineStatusHistory.created_by = userId;
                        DARedlineStatusHistory.Instance.InsertHistoryDetails(objRedlineStatusHistory);
                    }

                    return objRedlineMaster;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<RedlineStatusMaster> getStatusDropdown()
        {
            try
            {
                var lst = repo.ExecuteProcedure<RedlineStatusMaster>("fn_get_redline_status_dropdown", false);
                return lst;
            }
            catch (Exception e)
            {

                throw;
            }

        }
    }

    public class DARedlineAssignedUsers : Repository<RedlineAssignedUsers>
    {

        private static DARedlineAssignedUsers objAssignedUsers = null;
        private static readonly object lockObject = new object();
        public static DARedlineAssignedUsers Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objAssignedUsers == null)
                    {
                        objAssignedUsers = new DARedlineAssignedUsers();
                    }
                }
                return objAssignedUsers;
            }
        }
        public RedlineAssignedUsers InsertUserDetails(RedlineAssignedUsers objAssignedUsers)
        {
            try
            {
                return repo.Insert(objAssignedUsers);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public RedlineAssignedUsers UpdateUserDetails(RedlineAssignedUsers objAssignedUsers)
        {
            try
            {
                return repo.Update(objAssignedUsers);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public RedlineAssignedUsers GetAssignedUsersById(int task_system_id, int assigned_user)
        {
            try
            {
                return repo.Get(u => u.task_id == task_system_id && u.user_id == assigned_user);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    public class DARedlineStatusHistory : Repository<RedlineStatusHistory>
    {

        private static DARedlineStatusHistory objStatusHistory = null;
        private static readonly object lockObject = new object();
        public static DARedlineStatusHistory Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objStatusHistory == null)
                    {
                        objStatusHistory = new DARedlineStatusHistory();
                    }
                }
                return objStatusHistory;
            }
        }
        public RedlineStatusHistory InsertHistoryDetails(RedlineStatusHistory objRedlineStatusHistory)
        {
            try
            {
                return repo.Insert(objRedlineStatusHistory);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}