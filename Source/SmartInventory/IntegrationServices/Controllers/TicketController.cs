using BusinessLogics;
using BusinessLogics.WFM;
using IntegrationServices.Filters;
using IntegrationServices.Settings;
using Models;
using Models.API;
using Models.WFM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Http;
using Utility;

namespace IntegrationServices.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1")]
    public class TicketController : ApiController
    {
        [HttpPost]
        [Route("getAppointmentSlots")]
        [ValidateModel]
        public WFMApiResponse<dynamic> getAppointmentSlots(SlotRequest slotRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    //SlotRequest obRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<SlotRequest>(inpuRequest.slotrequest.ToString());
                    //validate date
                    if (slotRequest.appointment_date < DateTime.Now.Date)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Appointment date is less than current date.";
                        return response;
                    }

                    // add main service on first in array
                    var service_facility = BLWFMTicket.GetMainServiceFacility();
                    var first_service_facility = service_facility.Where(j => j.service_facility_code == slotRequest.service_types[0]).ToList();
                    if (first_service_facility.Count > 0)
                    {
                        slotRequest.service_type = string.Join(",", slotRequest.service_types.ToArray());
                    }
                    else
                    {
                        String mainServices = "";
                        foreach (var item in slotRequest.service_types)
                        {
                            var service_main_facility = service_facility.Where(j => j.service_facility_code == item).ToList();
                            if (service_main_facility.Count > 0)
                            {
                                mainServices = item;
                                break;
                            }
                        }
                        slotRequest.service_type = string.Join(",", slotRequest.service_types.ToArray());
                        slotRequest.service_type = slotRequest.service_type.Replace("," + mainServices, "");
                        slotRequest.service_type = mainServices + "," + slotRequest.service_type;
                    }


                    var iscancelled = BLWFMTicket.GetSlotConfirmationByRefId(slotRequest.referenceid, 1);
                    //Array.Reverse(slotRequest.service_type);
                    if (iscancelled != null)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Slot already cancelled of requested reference id";
                        return response;
                    }

                    //for store request in db
                    var request = Newtonsoft.Json.JsonConvert.SerializeObject(slotRequest);
                    slotRequest.request = request;

                    var result = BLWFMTicket.GetSlotConfirmationByRefId(slotRequest.referenceid, 0);
                    if (result != null)
                    {

                        var cnfBookin = BLWFMTicket.isbookingIdExist(result.bookingid);
                        if (cnfBookin)
                        {
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.message = "Can not get slot of requested reference id, because job order is created";
                            return response;
                        }
                        //response.status = StatusCodes.INVALID_INPUTS.ToString();
                        //response.message = "Slot already reserved of requested reference id";
                        //return response;
                        //    SlotRequest OldSlotRequest = BLWFMTicket.GetSlotRequest(slotRequest.referenceid);


                        //    //Rule 2
                        //    string message;
                        //    string joType;
                        //    string add_service;
                        //    if (!ValidateJoServiceType(OldSlotRequest, out message, out joType, out add_service))
                        //    {
                        //        response.status = StatusCodes.FAILED.ToString();
                        //        response.message = message;
                        //        return response;
                        //    }

                        //    var JoRoleMapping = BLWFMTicket.GetRoleNameByJoCategory(OldSlotRequest.jo_category, joType);
                        //    string fe_type = JoRoleMapping.workflow == "Regular process" ? "regular" : "fastlane";

                        //    string femessage;
                        //    int slotId;
                        //    List<FEList> AVFEList;
                        //    if (!GetEFList(slotRequest, out slotId, out AVFEList, OldSlotRequest.managerid, out femessage, joType, add_service, fe_type))
                        //    {
                        //        response.status = StatusCodes.FAILED.ToString();
                        //        response.message = femessage;
                        //        return response;
                        //    }


                        //    #region Check roster

                        //    //string strTimeSheetError = "";
                        //    DateTime dtCurDate = DateTime.Now;//
                        //    int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                        //    AVFEList.ForEach(r =>
                        //    {
                        //        string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                        //        //bool isTimeSheet = BLWFMTicket.isTimeSheetDefined(r.UserId ?? 0, DateTime.Now, intDayOfWeek, strTime, out strTimeSheetError);
                        //        var isTimeSheet = BLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, slotRequest.appointment_date, intDayOfWeek, strTime);

                        //        if (isTimeSheet != null)
                        //        {
                        //            r.isRosterAvailable = true;
                        //            r._start_time = isTimeSheet._start_time;
                        //            r._end_time = isTimeSheet._end_time;
                        //        }
                        //        else
                        //        {
                        //            r.isRosterAvailable = false;
                        //        }
                        //    });

                        //    if (AVFEList.Count > 0 && !AVFEList.Any(a => a.isRosterAvailable == true))
                        //    {
                        //        response.status = StatusCodes.FAILED.ToString();
                        //        response.message = AVFEList.Count + " FEs found but no roster available";
                        //        return response;
                        //    }
                        //    var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

                        //    #endregion

                        //    //get roster start and end time from available fe
                        //    int rosterStartTime = AvailableEF.Min(a => a._start_time);//10;
                        //    int rosterEndTime = AvailableEF.Max(a => a._end_time);// 20;

                        //    var SlotList = BLWFMTicket.GetSlot(slotId, rosterStartTime, rosterEndTime, slotRequest.appointment_date, slotRequest.referenceid, AVFEList.Count, OldSlotRequest.managerid);
                        //    response.status = StatusCodes.OK.ToString();
                        //    response.results = SlotList;
                    }
                    //else
                    //{
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(slotRequest.referenceid);
                    //int id = 0;
                    if (SlotRequest != null)
                    {

                        BLWFMTicket.updateSlotRequest(slotRequest);
                        //response.status = StatusCodes.FAILED.ToString();
                        //response.message = "reference id already exists.";
                        //return response;
                    }
                    else
                    {

                        //insert slot request
                        slotRequest.created_date = DateTime.Now;
                        BLWFMTicket.InsertSlotRequest(slotRequest);
                    }

                    //1.Save Slot Request
                    //2.Find manager from coordinates(112)
                    //3.Find FE for above manager(101, 102)
                    //4.Filter FE as per ticket type(1, 3, 4), their skill, their roster(9 - 6)
                    //5.Do slot calculation and return slots with slot id


                    ////Rule 2
                    //var JoRoleMapping = BLWFMTicket.GetRoleNameByJoCategory(slotRequest.jo_category);
                    //if (JoRoleMapping == null)
                    //{
                    //    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    //    response.message = string.Format("Role not found on selected Jo category : {0}", slotRequest.jo_category);
                    //    return response;
                    //}
                    //var roleName = JoRoleMapping.role_name;


                    ////Rule 2
                    string message;
                    string joType;
                    string add_service;
                    if (!ValidateJoServiceType(slotRequest, out message, out joType, out add_service))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = message;
                        slotRequest.response = message;
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                        return response;
                    }
                    //Rule 3
                    var JoRoleMapping = BLWFMTicket.GetRoleNameByJoCategory(slotRequest.jo_category, joType);
                    if (JoRoleMapping == null)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = string.Format("Role not found on selected Jo category : {0}", slotRequest.jo_category);
                        //slotRequest.id = id;
                        slotRequest.response = message;
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                        return response;
                    }
                    var roleName = JoRoleMapping.role_name;
                    string fe_type = JoRoleMapping.workflow == "Regular process" ? "regular" : "fastlane";
                    string[] rolesList = roleName.Split(',');
                    string blockName = BLWFMTicket.GetBlockName(Convert.ToDouble(slotRequest.latitude), Convert.ToDouble(slotRequest.longitude));
                    if (String.IsNullOrEmpty(blockName))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "No barangay area found in requested lat lng";
                        slotRequest.response = response.message;
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                        return response;
                    }
                    var Manager = new List<ManagerList>();
                    int managerUserId = 0;
                    //var ManagerName = Manager.FirstOrDefault().user_name;
                    string ManagerName = "";
                    string responseData = "";
                    //foreach (string role in rolesList)
                    //{
                    //    Manager = BLWFMTicket.GetManagerList(Convert.ToDouble(slotRequest.latitude), Convert.ToDouble(slotRequest.longitude), role);
                    //    CurrentRole = role;
                    //    if (Manager != null)
                    //    {
                    //        break;
                    //    }
                    //}
                    string AllRoleList = String.Join("','", rolesList);
                    Manager = BLWFMTicket.GetAllManagerList(Convert.ToDouble(slotRequest.latitude), Convert.ToDouble(slotRequest.longitude), AllRoleList);
                    if (Manager.Count == 0)
                    {
                        //slotRequest.id = id;
                        slotRequest.response = roleName + "  not found in " + blockName + " barangay";
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = roleName + " not found in " + blockName + " barangay";
                        return response;
                    }
                    //case 1 if Manager.Count == 0 then no user found
                    // var Manager = BLWFMTicket.GetManagerList( Convert.ToDouble(slotRequest.latitude), Convert.ToDouble(slotRequest.longitude), JoRoleMapping.role_name);
                    else if (Manager.Count > 0)
                    {
                        var isrecord = Manager.Any(m => m.role_name == null);
                        if (isrecord)
                        {
                            //slotRequest.id = id;
                            slotRequest.response = roleName + " not found in " + Manager.FirstOrDefault().block_name + " barangay";
                            BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                            response.status = StatusCodes.FAILED.ToString();
                            response.message = roleName + " not found in " + Manager.FirstOrDefault().block_name + " barangay";
                            return response;
                        }
                    }

                    //case 4
                    var _isUserjotype = Manager.Where(j => j.user_Jo_type == joType).ToList();
                    if (_isUserjotype.Count == 0)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = string.Format("User found but not map with {0} jo type", joType);
                        //slotRequest.id = id;
                        slotRequest.response = message;
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                        return response;
                    }
                    //case 5
                    var _isUserService = _isUserjotype.Where(j => j.user_service == add_service).ToList();
                    if (_isUserService.Count == 0)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = string.Format("User found but not map with {0} service", add_service);
                        // slotRequest.id = id;
                        slotRequest.response = message;
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                        return response;
                    }
                    //6
                    var _jo_category_user = _isUserService.Where(j => j.jo_category_user.ToUpper() == slotRequest.jo_category.ToUpper()).ToList();
                    if (_jo_category_user.Count == 0)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = string.Format("User found but not map with {0} jo category", slotRequest.jo_category);
                        // slotRequest.id = id;
                        slotRequest.response = message;
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);
                        return response;
                    }
                    List<FRTCapacity> dispatcherCountDetail = new List<FRTCapacity>();
                    List<ManagerList> allmgr = new List<ManagerList>();
                    string dispatcherCountString = "";
                    foreach (var item in _jo_category_user)
                    {
                        UserDetails userDetails = BLWFMTicket.GetUserRoleDetailsById(Convert.ToInt32(item.user_id));
                        //  User_Master userDetails = BLWFMTicket.GetUserById(Convert.ToInt32(item.user_id));

                        dispatcherCountDetail = BLWFMTicket.GetDispatcherCountDetail(userDetails.manager_id, slotRequest.appointment_date);
                        item.dispatchercount = dispatcherCountDetail.Count;
                        item.group_name = userDetails.manager_name;
                        allmgr.Add(item);
                        //dispatcherCountString = dispatcherCountString +item.user_name+ ": "+item.dispatchercount+"  ("+ userDetails.manager_name +" : "+ userDetails.manager_role_name+" ), ";
                        dispatcherCountString = dispatcherCountString + item.user_name + ":" + userDetails.role_name + ":" + userDetails.manager_name + ":" + userDetails.manager_role_name + ":" + item.dispatchercount + ", ";
                    }
                    allmgr = allmgr.OrderBy(j => j.group_name).ToList();
                    allmgr = allmgr.OrderBy(j => j.dispatchercount).ToList();
                    List<SlotResponse> slotResponses = new List<SlotResponse>();
                    foreach (var item in allmgr)
                    {
                        managerUserId = item.user_id ?? 0;
                        ManagerName = item.user_name;
                        //slotRequest.id = id;
                        //slotRequest.managerid = managerUserId;
                        slotRequest.remark = dispatcherCountString;
                        BLWFMTicket.UpdateSlotRequesNew(slotRequest);

                        string femessage;
                        int slotId;
                        List<FEList> AVFEList;
                        if (!GetEFList(slotRequest, out slotId, out AVFEList, managerUserId, out femessage, joType, add_service, fe_type))
                        {
                            response.status = StatusCodes.FAILED.ToString();
                            response.message = femessage;
                            //return response;
                            //slotRequest.id = id;
                            responseData += response.message + System.Environment.NewLine;
                            // slotRequest.response += response.message +", ";
                            //BLWFMTicket.InsertSlotRequest(slotRequest);
                            continue;
                        }

                        #region Check roster

                        //string strTimeSheetError = "";
                        DateTime dtCurDate = DateTime.Now;//
                        int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                        AVFEList.ForEach(r =>
                        {
                            string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                            //bool isTimeSheet = BLWFMTicket.isTimeSheetDefined(r.UserId ?? 0, DateTime.Now, intDayOfWeek, strTime, out strTimeSheetError);
                            var isTimeSheet = BLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, slotRequest.appointment_date, intDayOfWeek, strTime);

                            if (isTimeSheet != null)
                            {
                                r.isRosterAvailable = true;
                                r._start_time = isTimeSheet._start_time;
                                r._end_time = isTimeSheet._end_time;
                            }
                            else
                            {
                                r.isRosterAvailable = false;
                            }
                        });

                        if (AVFEList.Count > 0 && !AVFEList.Any(a => a.isRosterAvailable == true))
                        {
                            response.status = StatusCodes.FAILED.ToString();
                            response.message = AVFEList.Count + " FEs found but no roster available" + ": " + ManagerName;
                            //ErrorLogHelper logHelper = new ErrorLogHelper();
                            //logHelper.ApiLogWriter("getAppointmentSlots()", "TicketController", response.message + ": " + ManagerName, null);
                            //slotRequest.id = id;
                            responseData += response.message  +System.Environment.NewLine;
                            continue;
                            // slotRequest.response += response.message + ", ";
                            //BLWFMTicket.InsertSlotRequest(slotRequest);
                            //return response;

                        }
                        else
                        {
                            var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

                            #endregion
                            int rosterStartTime = AvailableEF.Min(a => a._start_time);
                            int rosterEndTime = AvailableEF.Max(a => a._end_time);
                            var SlotList = BLWFMTicket.GetSlot(slotId, rosterStartTime, rosterEndTime, slotRequest.appointment_date, slotRequest.referenceid, AvailableEF.Count, AvailableEF, managerUserId, ManagerName);
                            if (SlotList.Count == 0)
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.message = "No slot available for " + ManagerName + " ,";
                                //slotRequest.id = id;
                                responseData += response.message  +System.Environment.NewLine;
                                continue;
                                //  slotRequest.response += response.message+", ";
                                //  BLWFMTicket.InsertSlotRequest(slotRequest);
                                //return response;
                            }
                            else
                            {
                                var totalSlot = BLWFMTicket.GetAllFreeSlot(managerUserId, slotRequest.appointment_date).Count();
                                if (item.capacity > 0)
                                {
                                    if (totalSlot > item.capacity)
                                    {
                                        response.status = StatusCodes.OK.ToString();
                                        response.message = "Maximum capacity of the " + ManagerName + " is full.,";
                                        // slotRequest.response += response.message + ", ";
                                        //slotRequest.id = id;
                                        responseData += response.message + System.Environment.NewLine;
                                        continue;
                                        //BLWFMTicket.InsertSlotRequest(slotRequest);
                                    }
                                    else
                                    {
                                        response.status = StatusCodes.OK.ToString();
                                        //response.results = SlotList;
                                        response.message = "Slot found with " + ManagerName;// + ", Role Name: "+roleName;
                                        //slotRequest.id = id;
                                        slotRequest.managerid = managerUserId;
                                        foreach (var sList in SlotList)
                                        {
                                            string tempslotid = sList.temp_slotid;
                                            var isExistSlotId = slotResponses.Where(f => f.temp_slotid == tempslotid).ToList();
                                            if (isExistSlotId.Count == 0)
                                            {
                                                slotResponses.Add
                                         (
                                                 new SlotResponse()
                                                 {
                                                     slotid = sList.slotid,
                                                     temp_slotid = sList.temp_slotid,
                                                     from_time = sList.from_time,
                                                     to_time = sList.to_time,
                                                     referenceId = sList.referenceId,
                                                     appointment_date = sList.appointment_date,
                                                     dispatcher_name = sList.dispatcher_name,
                                                 }
                                            );
                                            }
                                        }
                                        response.results = slotResponses;
                                        responseData += "Slot found with " + ManagerName + System.Environment.NewLine;
                                        //slotRequest.response = responseData;
                                        //int updateid = BLWFMTicket.InsertSlotRequest(slotRequest);
                                        //break;
                                    }
                                }
                                else
                                {
                                    response.status = StatusCodes.OK.ToString();
                                  //  response.results = SlotList;
                                    response.message = "Slot found with " + ManagerName;// + ", Role Name: " + roleName;
                                                                                        // slotRequest.id = id;
                                    slotRequest.managerid = managerUserId;
                                    // slotRequest.response += ManagerName+":"+Newtonsoft.Json.JsonConvert.SerializeObject(SlotList);                     
                                  //  responseData += ManagerName + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(SlotList);
                                    foreach (var sList in SlotList)
                                    {
                                        string tempslotid = sList.temp_slotid;
                                        var isExistSlotId = slotResponses.Where(f => f.temp_slotid == tempslotid).ToList();
                                        if (isExistSlotId.Count == 0)
                                        {
                                            slotResponses.Add
                                                (
                                                new SlotResponse()
                                                {
                                                    slotid = sList.slotid,
                                                    temp_slotid = sList.temp_slotid,
                                                    from_time = sList.from_time,
                                                    to_time = sList.to_time,
                                                    referenceId = sList.referenceId,
                                                    appointment_date = sList.appointment_date,
                                                    dispatcher_name = sList.dispatcher_name,
                                                }
                                                );
                                        }
                                    }
                                    response.results = slotResponses;
                                    responseData += "Slot found with " + ManagerName + System.Environment.NewLine;
                                    //slotRequest.response = responseData;
                                    //int updateid = BLWFMTicket.InsertSlotRequest(slotRequest);
                                    // break;
                                }

                            }
                        }

                        // return response;

                        //managerUserId = _isUserService.FirstOrDefault().user_id ?? 0;
                        //ManagerName = _isUserService.FirstOrDefault().user_name;
                        //slotRequest.id = id;
                        //slotRequest.managerid = managerUserId;
                        //BLWFMTicket.InsertSlotRequest(slotRequest);

                        //string femessage;
                        //int slotId;
                        //List<FEList> AVFEList;
                        //if (!GetEFList(slotRequest, out slotId, out AVFEList, managerUserId, out femessage, joType, add_service, fe_type))
                        //{
                        //    response.status = StatusCodes.FAILED.ToString();
                        //    response.message = femessage;
                        //    return response;
                        //}

                        //#region Check roster

                        ////string strTimeSheetError = "";
                        //DateTime dtCurDate = DateTime.Now;//
                        //int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                        //AVFEList.ForEach(r =>
                        //{
                        //    string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                        //    //bool isTimeSheet = BLWFMTicket.isTimeSheetDefined(r.UserId ?? 0, DateTime.Now, intDayOfWeek, strTime, out strTimeSheetError);
                        //    var isTimeSheet = BLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, slotRequest.appointment_date, intDayOfWeek, strTime);

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
                        //    response.status = StatusCodes.FAILED.ToString();
                        //    response.message = AVFEList.Count + " FEs found but no roster available";
                        //    return response;
                        //}
                        //var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

                        //#endregion

                        //int rosterStartTime = AvailableEF.Min(a => a._start_time);
                        //int rosterEndTime = AvailableEF.Max(a => a._end_time);

                        //var SlotList = BLWFMTicket.GetSlot(slotId, rosterStartTime, rosterEndTime, slotRequest.appointment_date, slotRequest.referenceid, AvailableEF.Count, managerUserId);

                        //if (SlotList.Count == 0)
                        //{
                        //    response.status = StatusCodes.INVALID_INPUTS.ToString();
                        //    response.message = "No slot available";
                        //    return response;
                        //}
                        //response.status = StatusCodes.OK.ToString();
                        //response.results = SlotList;
                        //slotRequest.id = id;
                        //slotRequest.response = Newtonsoft.Json.JsonConvert.SerializeObject(SlotList);
                        //int updateid = BLWFMTicket.InsertSlotRequest(slotRequest);

                    }
                    if(response.results!=null)
                    {
                        response.message =  "Slot found ";
                        response.status = StatusCodes.OK.ToString();
                    }
                    //slotRequest.id = id;
                    slotRequest.response = responseData;
                    BLWFMTicket.UpdateSlotRequesNew(slotRequest);

                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var request = Newtonsoft.Json.JsonConvert.SerializeObject(slotRequest);
                    logHelper.ApiLogWriter("getAppointmentSlots()", "TicketController", request, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }

            }

            //else
            //{
            //    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            //    response.message = "Invalid format.";
            //}
            return response;
        }
        public static bool GetEFList(SlotRequest obRequest, out int slotId, out List<FEList> AVFEList, int managerUserId, out string message, string joType, string add_service, string fe_type = null)
        {
            bool isValidate = false;
            slotId = 0;
            message = "";
            AVFEList = new List<FEList>();
            User_Master usr_mstr= BLWFMTicket.GetUserById(managerUserId);
            string ManagerName = usr_mstr.user_name;
            //string ManagerName = MiscHelper.Decrypt(BLWFMTicket.GetUserNameById(managerUserId));

            var FEList = BLWFMTicket.GetFEList(managerUserId).Where(f => f.user_type == fe_type).ToList();
            if (FEList.Count == 0)
            {
                message = string.Format("No FE map with dispatcher {0}", ManagerName);
                return isValidate;
            }
            //case 6
            var _isUserjotype = FEList.Where(j => j.user_Jo_type == joType).ToList();
            if (FEList.Count == 0)
            {
                message = string.Format("{0}, FE user found but not map with {1} jo type", ManagerName, joType);
                return isValidate;
            }
            //case 7
            var _isUserService = _isUserjotype.Where(j => j.user_service == add_service).ToList();
            if (_isUserService.Count == 0)
            {
                message = string.Format("{0}, FE user found but not map with {1} service", ManagerName, add_service);
                return isValidate;
            }

            //get skill id and slot id
            //var servicetype = BLWFMTicket.Getservicetype(obRequest.service_type);
            //slotId = servicetype.sdid;
            //if (slotId == 0)
            //{
            //    message = string.Format("{0}, FE user found but slot not map with {0} service type", ManagerName, servicetype.add_service);
            //    return isValidate;
            //}

            //Rule 3 
            var service = obRequest.service_type;

            int slot_duration = 0;
            if (service.Contains(","))
            {
                string[] services = service.Split(',');
                foreach (var x in services)
                {
                    var service_facility = BLWFMTicket.GetServiceFacility(x);
                    if (service_facility == null)
                    {
                        message = string.Format("Service facility {0} not found", x);
                        return isValidate;
                    }
                    slot_duration += service_facility.slot_duration;
                }
            }
            else
            {
                var service_facility = BLWFMTicket.GetServiceFacility(service);
                if (service_facility == null)
                {
                    message = string.Format("Service facility {0} not found", service);
                    return isValidate;
                }
                slot_duration = service_facility.slot_duration;
            }


            if (slot_duration == 0)
            {
                message = string.Format("Slot duration not map with service facility.");
                return isValidate;
            }
            var slotDuration = BLWFMTicket.GetSlotDurationDetails(slot_duration);

            if (slotDuration == null)
            {
                message = string.Format("Slot duration {0} not found", slot_duration);
                return isValidate;
            }

            slotId = slotDuration.sdid;


            //string skillid = servicetype.skid;
            #region skill not map to db so comment
            //get skilled ef list
            //var SkillFE = BLWFMTicket.GetSkillFE(skillid);
            // get available fe list
            //AVFEList = BLWFMTicket.GetAVFE(FEList, SkillFE);
            #endregion

            AVFEList = _isUserService;
            isValidate = true;
            return isValidate;
        }


        private static bool ValidateJoServiceType(SlotRequest obRequest, out string message, out string jo_type, out string service)
        {
            bool isValidate = false;
            message = "";
            jo_type = "";
            service = "";
            #region Validate jo type and service
            //Rule 2
            //var orderType = BLWFMTicket.GetOrderType(obRequest.order_type);

            //if (orderType == null)
            //{
            //    message = string.Format("Order type not found");
            //    return isValidate;
            //}

            //if (string.IsNullOrEmpty(orderType.description2))
            //{
            //    message = string.Format("Jo type not map with selected order type : {0}", obRequest.order_type);
            //    return isValidate;
            //}
            //jo_type = orderType.description2;

            //Rule 3 changes in get slot request

            var joType = BLWFMTicket.GetJoType(obRequest.order_type); //New Installation

            if (joType == null)
            {
                message = string.Format("Order type not found");
                return isValidate;
            }
            jo_type = obRequest.order_type;


            //var serviceType = BLWFMTicket.Getservicetype(obRequest.service_type);

            //if (serviceType == null)
            //{
            //    message = string.Format("Service type not found");
            //    return isValidate;
            //}

            //if (string.IsNullOrEmpty(serviceType.add_service))
            //{
            //    serviceType.add_service = serviceType.remove_service;
            //}
            //if (string.IsNullOrEmpty(serviceType.add_service))
            //{
            //    message = string.Format("Add/Remove service facility  not map with selected service type : {0}", obRequest.service_type);
            //    return isValidate;
            //}
            service = obRequest.service_type;

            if (service.Contains(","))
            {
                service = service.Split(',').First();
            }

            isValidate = true;
            #endregion
            return isValidate;
        }

        [HttpPost]
        [Route("reserveAppointmentSlot")]
        [ValidateModel]
        public WFMApiResponse<dynamic> reserveAppointmentSlot(SlotConfirmation obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    //

                    var iscancelled = BLWFMTicket.GetSlotConfirmationByRefId(obRequest.referenceid, 1);
                    if (iscancelled != null)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Slot already cancelled of requested reference id";
                        return response;
                    }

                    var rsCkeckslot = BLWFMTicket.GetSlotConfirmationByRefId(obRequest.referenceid, 0);
                    if (rsCkeckslot != null)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Slot already reserved of requested reference id";
                        return response;
                    }
                    obRequest.created_date = DateTime.Now.Date;
                    obRequest.created_datetime = DateTime.Now;
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(obRequest.referenceid);
                    int slotManagerId = Convert.ToInt32(obRequest.slotid.Split('_')[1]); 
                    SlotRequest.managerid = slotManagerId;
                    //obRequest.slotid = actualSlotId;
                    DateTime appointment_date;
                    int maste_slot_id;
                    BLWFMTicket.GetValue(obRequest.slotid, out appointment_date, out maste_slot_id);
                    obRequest.appointment_date = appointment_date;
                    obRequest.master_slot_id = maste_slot_id;
                    if (SlotRequest == null)
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.message = "Reference id not found";
                        return response;
                    }
                    #region Validate jo type and service
                    //Rule 2
                    string message;
                    string joType;
                    string add_service;
                    if (!ValidateJoServiceType(SlotRequest, out message, out joType, out add_service))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = message;
                        return response;
                    }

                    var JoRoleMapping = BLWFMTicket.GetRoleNameByJoCategory(SlotRequest.jo_category, joType);
                    string fe_type = JoRoleMapping.workflow == "Regular process" ? "regular" : "fastlane";

                    #endregion

                    string femessage;
                    int slotId;
                    List<FEList> AVFEList;
                    if (!GetEFList(SlotRequest, out slotId, out AVFEList, SlotRequest.managerid, out femessage, joType, add_service, fe_type))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = femessage;
                        return response;
                    }

                    #region Check roster

                    //string strTimeSheetError = "";
                    DateTime dtCurDate = DateTime.Now;//
                    int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                    AVFEList.ForEach(r =>
                    {
                        string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                        //bool isTimeSheet = BLWFMTicket.isTimeSheetDefined(r.UserId ?? 0, DateTime.Now, intDayOfWeek, strTime, out strTimeSheetError);
                        var isTimeSheet = BLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, SlotRequest.appointment_date, intDayOfWeek, strTime);

                        if (isTimeSheet != null)
                        {
                            r.isRosterAvailable = true;
                            r._start_time = isTimeSheet._start_time;
                            r._end_time = isTimeSheet._end_time;
                        }
                        else
                        {
                            r.isRosterAvailable = false;
                        }
                    });

                    if (AVFEList.Count > 0 && !AVFEList.Any(a => a.isRosterAvailable == true))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = AVFEList.Count + " FE found but no roster available";
                        return response;
                    }
                    var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

                    #endregion
                  string actualSlotId= obRequest.slotid.Split('_')[0];
                    //var slot = BLWFMTicket.GetFreeSlot(obRequest.slotid, SlotRequest.managerid);
                    var master_slot_id = actualSlotId.Length == 10 ? Convert.ToInt32(actualSlotId.Substring(8, 2)) : Convert.ToInt32(actualSlotId.Substring(8, 1));

                    //Change
                    int appcount = 0;
                    var slotdetails = BLWFMTicket.GetSlotDetailById(master_slot_id);

                    var getSlots = BLWFMTicket.GetAllFreeSlot(SlotRequest.managerid, SlotRequest.appointment_date);

                    if (getSlots.Count > 0)
                    {
                        appcount = getSlots.Where(w => w.from_time == slotdetails.from_time).Count();

                        if (appcount < 0)
                        {
                            appcount = getSlots.Where(w => w.to_time == slotdetails.to_time).Count();
                        }

                    }
                    //Change

                    int totalslot = AvailableEF.Count - appcount;

                    //set manager userid
                    obRequest.managerid = SlotRequest.managerid;
                    if (totalslot > 0)
                    {
                        int bookingId = BLWFMTicket.InsertSlotConfimation(obRequest);
                        if (bookingId > 0)
                        {
                            BookingResponce resbokking = new BookingResponce() { bookingid = bookingId };
                            response.status = "200";
                            response.message = "Slot reserved successfully";
                            response.results = resbokking;
                        }
                    }
                    else
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.message = "Sorry ! Slot booked";
                    }

                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                    logHelper.ApiLogWriter("reserveAppointmentSlot()", "TicketController", request, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }
            //else
            //{
            //    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            //    response.message = "Invalid format.";
            //}
            return response;
        }

        //createJobOrder
        [HttpPost]
        [Route("createJobOrder")]
        [ValidateModel]
        public WFMApiResponse<dynamic> createJobOrder(CreateJob obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            try
            {
                if (ModelState.IsValid)
                {
                    // check job id already created or not
                    var isjobId = BLWFMTicket.isJobIdExist(obRequest.joborderid);
                    if (isjobId)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "joborderid " + obRequest.joborderid + " already created.";
                        return response;
                    }

                    var isbookingid = BLWFMTicket.isbookingIdExist(obRequest.bookingid);
                    if (isbookingid)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "bookingid " + obRequest.bookingid + " already created.";
                        return response;
                    }

                    //check booking id available or not
                    SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(obRequest.bookingid);
                    if (Request == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "booking id not found";
                        return response;
                    }
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(Request.referenceid);
                    if (SlotRequest == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Reference id not found";
                        return response;
                    }

                    //validate date
                    if (SlotRequest.appointment_date < DateTime.Now.Date)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "appointment date is less than current date.";
                        return response;
                    }

                    //Address
                    obRequest.address.ForEach(f =>
                    {
                        obRequest.address_line1 = f.address_line1;
                        obRequest.address_line2 = f.address_line2;
                        obRequest.address_line3 = f.address_line3;
                        obRequest.address_type = f.address_type;
                        obRequest.city = f.city;
                        obRequest.pinCode = f.pincode;
                        obRequest.state_Province = f.stateorprovince;
                        obRequest.address_id = f.addressid;

                    });

                    //var serviceType = BLWFMTicket.Getservicetype(SlotRequest.service_type);
                    string managerName = MiscHelper.Decrypt(BLWFMTicket.GetUserNameById(SlotRequest.managerid));
                    //if (serviceType == null)
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = "Service type not found";
                    //    return response;
                    //}
                    //if (string.IsNullOrEmpty(serviceType.add_service) && string.IsNullOrEmpty(serviceType.remove_service))
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = string.Format("Add/Remove service facility not map with service type : {0}", SlotRequest.service_type);
                    //    return response;
                    //}

                    #region Create job order for add service/remove service
                    int jobIdSequence = 1;
                    List<AddRemoveservice> AddService = new List<AddRemoveservice>();
                    //if (!string.IsNullOrEmpty(serviceType.add_service))
                    //{
                    //    if (serviceType.add_service.Contains(","))
                    //    {
                    //        serviceType.add_service.Split(',').ToList().ForEach(x =>
                    //        {
                    //            AddService.Add(new AddRemoveservice() { service = x, action = "Add" });
                    //        });
                    //    }
                    //    else
                    //    {
                    //        AddService.Add(new AddRemoveservice() { service = serviceType.add_service, action = "Add" });
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(serviceType.remove_service))
                    //{
                    //    if (serviceType.remove_service.Contains(","))
                    //    {
                    //        serviceType.remove_service.Split(',').ToList().ForEach(x =>
                    //        {
                    //            AddService.Add(new AddRemoveservice() { service = x, action = "Remove" });
                    //        });
                    //    }
                    //    else
                    //    {
                    //        AddService.Add(new AddRemoveservice() { service = serviceType.add_service, action = "Remove" });
                    //    }
                    //}

                    if (SlotRequest.service_type.Contains(","))
                    {
                        SlotRequest.service_type.Split(',').ToList().ForEach(x =>
                        {
                            AddService.Add(new AddRemoveservice() { service = x, action = "Add" });
                        });
                    }
                    else
                    {
                        AddService.Add(new AddRemoveservice() { service = SlotRequest.service_type, action = "Add" });
                    }







                    //create job order for add service
                    foreach (var service in AddService)
                    {
                        //create job order id
                        string parentJobOrderId = obRequest.joborderid;
                        string jobOrderId = AddService.Count == 1 ? obRequest.joborderid : obRequest.joborderid + "-" + jobIdSequence;

                        //create job order
                        var res = CreateTicket(obRequest, SlotRequest, jobOrderId, parentJobOrderId, service.action, service.service);
                        if (res.status != StatusCodes.OK.ToString())
                        {
                            response.status = res.status;
                            response.message = res.message;
                            return response;
                        }
                        response.status = res.status;
                        //response.message = res.message + " ,assign to contractor : " + managerName;
                        response.message = string.Concat(res.message, " ,assign to contractor : ", managerName);
                        jobIdSequence++;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                logHelper.ApiLogWriter("createJobOrder()", "TicketController", request, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.message = "Error While Processing  Request.";
            }
            return response;
        }

        public WFMApiResponse<dynamic> CreateTicket(CreateJob obRequest, SlotRequest SlotRequest, string jobOrderId, string parentJobOrderId, string serviceType, string service)
        {
            var response = new WFMApiResponse<dynamic>();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string strMessage = "";
            string strStatus = "";
            int ticket_tran_id = 0;

            Route_Issue objRouteIssue = new Route_Issue();
            if (!CreateHPSMTicket(obRequest, SlotRequest, ref strMessage, ref strStatus, ref ticket_tran_id, jobOrderId, parentJobOrderId, serviceType, service))
            {
                response.message = strMessage + " Ticket:" + obRequest.joborderid;
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                //ErrorLogHelper logHelper = new ErrorLogHelper();
                //logHelper.ApiLogWriter("CreateHPSMTicketfor()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                return response;
            }
            int route_issue_id = 0;
            if (!CreateRouteIssue(SlotRequest, ref objRouteIssue, ref strMessage, ref strStatus, ticket_tran_id, ref route_issue_id))
            {
                response.message = strMessage + " Ticket:" + obRequest.joborderid;
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                //ErrorLogHelper logHelper = new ErrorLogHelper();
                //logHelper.ApiLogWriter("CreateRouteIssueforNMS()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                return response;
            }
            ViewManagerRouteIssueApprove objMRIA = new ViewManagerRouteIssueApprove();
            // objMRIA.lstUserDetail = BLUser.GetSubordinateDetails(intManagerId, "FRT").ToList();//
            //objMRIA.issueId = objRouteIssue.issue_id;
            //objMRIA.issuesId = objRouteIssue.issue_id.ToString();
            //objMRIA.user_id = SlotRequest.managerid;
            //objMRIA.frtUserId = SlotRequest.managerid;
            //objMRIA.status = "UnAssigned";
            //objMRIA.remarks = "Assigned Ticket To Manager";
            //objMRIA.assignedDate = DateTime.Now;

            var autoAssignJobToFE = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoAssignJobToFE"]);
            if (autoAssignJobToFE)
            {
                var FEList = BLWFMTicket.GetFEList(SlotRequest.managerid);

                if (FEList.Count > 0)
                {
                    objMRIA.issueId = objRouteIssue.issue_id;
                    objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                    objMRIA.user_id = SlotRequest.managerid;
                    objMRIA.frtUserId = Convert.ToInt32(FEList.FirstOrDefault().UserId);
                    objMRIA.status = "Assigned";
                    objMRIA.remarks = "Automatically Assigned Ticket";
                    objMRIA.assignedDate = DateTime.Now;
                }
                else
                {
                    objMRIA.issueId = objRouteIssue.issue_id;
                    objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                    objMRIA.user_id = SlotRequest.managerid;
                    objMRIA.frtUserId = SlotRequest.managerid;
                    objMRIA.status = "UnAssigned";
                    objMRIA.remarks = "Assigned Ticket To Manager";
                    objMRIA.assignedDate = DateTime.Now;
                }
            }
            else
            {
                objMRIA.issueId = objRouteIssue.issue_id;
                objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                objMRIA.user_id = SlotRequest.managerid;
                objMRIA.frtUserId = SlotRequest.managerid;
                objMRIA.status = "UnAssigned";
                objMRIA.remarks = "Assigned Ticket To Manager";
                objMRIA.assignedDate = DateTime.Now;
            }
            if (!SaveRouteIssueStatus(objMRIA))
            {
                response.message = "Unable to Assign ticket to frt. Manager needs to assign ticket " + obRequest.joborderid + " to FE manually";
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                //ErrorLogHelper logHelper = new ErrorLogHelper();
                //logHelper.ApiLogWriter("CreateRouteIssueforNMS()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                return response;

            }
            else
            {
                try
                {
                    //NotificationHelper notificatonHelper = new NotificationHelper();
                    //var ids = objMRIA.issuesId.Split(',');
                    //for (int i = 0; i < ids.Length; i++)
                    //{
                    //    string strNotMess = "Task/Ticket id:" + objDataIn.ticket_id + " has been assigned.";
                    //    notificatonHelper.sendNotification(objMRIA.frtUserId, Convert.ToInt32(ids[i]), strNotMess, "Task/Ticket Assigned");
                    //}
                }
                catch (Exception ec)
                {
                    //ErrorLogHelper logHelper = new ErrorLogHelper();
                    //logHelper.ApiLogWriter("SaveRouteIssueStatus() + notificatonHelper", "Ticket Controller", JsonConvert.SerializeObject(objDataIn), ec);
                }
            }


            response.message = "Job order Created Successfully";
            response.status = StatusCodes.OK.ToString();

            return response;
        }

        public WFMApiResponse<dynamic> CreateTicketNew(CreateJobNew obRequest, SlotRequest SlotRequest, string jobOrderId, string parentJobOrderId, string serviceType, string service)
        {
            var response = new WFMApiResponse<dynamic>();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string strMessage = "";
            string strStatus = "";
            int ticket_tran_id = 0;

            Route_Issue objRouteIssue = new Route_Issue();
            if (!CreateHPSMTicketNew(obRequest, SlotRequest, ref strMessage, ref strStatus, ref ticket_tran_id, jobOrderId, parentJobOrderId, serviceType, service))
            {
                response.message = strMessage + " Ticket:" + obRequest.joborderid;
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                //ErrorLogHelper logHelper = new ErrorLogHelper();
                //logHelper.ApiLogWriter("CreateHPSMTicketfor()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                return response;
            }
            int route_issue_id = 0;
            if (!CreateRouteIssue(SlotRequest, ref objRouteIssue, ref strMessage, ref strStatus, ticket_tran_id, ref route_issue_id))
            {
                response.message = strMessage + " Ticket:" + obRequest.joborderid;
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                //ErrorLogHelper logHelper = new ErrorLogHelper();
                //logHelper.ApiLogWriter("CreateRouteIssueforNMS()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                return response;
            }
            if (!CreatePaymentDetails(obRequest, ref strMessage, ref strStatus, ticket_tran_id))
            {
                response.message = strMessage + " Ticket:" + obRequest.joborderid;
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                return response;
            }
            ViewManagerRouteIssueApprove objMRIA = new ViewManagerRouteIssueApprove();
            // objMRIA.lstUserDetail = BLUser.GetSubordinateDetails(intManagerId, "FRT").ToList();//
            //objMRIA.issueId = objRouteIssue.issue_id;
            //objMRIA.issuesId = objRouteIssue.issue_id.ToString();
            //objMRIA.user_id = SlotRequest.managerid;
            //objMRIA.frtUserId = SlotRequest.managerid;
            //objMRIA.status = "UnAssigned";
            //objMRIA.remarks = "Assigned Ticket To Manager";
            //objMRIA.assignedDate = DateTime.Now;

            var autoAssignJobToFE = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoAssignJobToFE"]);
            if (autoAssignJobToFE)
            {
                var FEList = BLWFMTicket.GetFEList(SlotRequest.managerid);

                if (FEList.Count > 0)
                {
                    objMRIA.issueId = objRouteIssue.issue_id;
                    objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                    objMRIA.user_id = SlotRequest.managerid;
                    objMRIA.frtUserId = Convert.ToInt32(FEList.FirstOrDefault().UserId);
                    objMRIA.status = "Assigned";
                    objMRIA.remarks = "Automatically Assigned Ticket";
                    objMRIA.assignedDate = DateTime.Now;
                }
                else
                {
                    objMRIA.issueId = objRouteIssue.issue_id;
                    objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                    objMRIA.user_id = SlotRequest.managerid;
                    objMRIA.frtUserId = SlotRequest.managerid;
                    objMRIA.status = "UnAssigned";
                    objMRIA.remarks = "Assigned Ticket To Manager";
                    objMRIA.assignedDate = DateTime.Now;
                }
            }
            else
            {
                objMRIA.issueId = objRouteIssue.issue_id;
                objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                objMRIA.user_id = SlotRequest.managerid;
                objMRIA.frtUserId = SlotRequest.managerid;
                objMRIA.status = "UnAssigned";
                objMRIA.remarks = "Assigned Ticket To Manager";
                objMRIA.assignedDate = DateTime.Now;
            }
            if (!SaveRouteIssueStatus(objMRIA))
            {
                response.message = "Unable to Assign ticket to frt. Manager needs to assign ticket " + obRequest.joborderid + " to FE manually";
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                //ErrorLogHelper logHelper = new ErrorLogHelper();
                //logHelper.ApiLogWriter("CreateRouteIssueforNMS()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                return response;

            }
            else
            {
                try
                {
                    //NotificationHelper notificatonHelper = new NotificationHelper();
                    //var ids = objMRIA.issuesId.Split(',');
                    //for (int i = 0; i < ids.Length; i++)
                    //{
                    //    string strNotMess = "Task/Ticket id:" + objDataIn.ticket_id + " has been assigned.";
                    //    notificatonHelper.sendNotification(objMRIA.frtUserId, Convert.ToInt32(ids[i]), strNotMess, "Task/Ticket Assigned");
                    //}
                }
                catch (Exception ec)
                {
                    //ErrorLogHelper logHelper = new ErrorLogHelper();
                    //logHelper.ApiLogWriter("SaveRouteIssueStatus() + notificatonHelper", "Ticket Controller", JsonConvert.SerializeObject(objDataIn), ec);
                }
            }

            var isSentMail = Convert.ToBoolean(ConfigurationManager.AppSettings["isSentMail"]);
            if (isSentMail)
            {
                //Send Email and Sms
                if (ticket_tran_id > 0)
                {
                    Models.WFM.Task JobOrder = BLWFMTicket.GetJobDetailByJobOrderId(obRequest.joborderid);
                    if (JobOrder == null)
                    {

                    }
                    //send email
                    SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(JobOrder.bookingid);
                    if (Request == null)
                    {

                    }
                    var result1 = BLWFMTicket.GetAppointmentDetails(Request.referenceid);
                    if (result1 != null)
                    {

                    }
                    var appointment_date = result1.appointment_date;
                    var from_time = result1.from_time;
                    var to_time = result1.to_time;

                    TimeSpan span = TimeSpan.FromHours(from_time);
                    DateTime time = DateTime.Today + span;
                    String sp1 = time.ToString("hh:mm tt");

                    TimeSpan span2 = TimeSpan.FromHours(to_time);
                    DateTime time2 = DateTime.Today + span2;
                    String sp2 = time2.ToString("hh:mm tt");
                    var ap_date = appointment_date + " " + sp1 + " to " + sp2;


                    var template = BLWFMTicket.GetTemplateDetail("Confirm-Schedule");
                    if (template != null)
                    {
                        var message = template.email.Replace("#date", ap_date);
                        var isMailsent = Email.sendMail("", JobOrder.email_id, template.subject, message);

                        var id = BLWFMTicket.InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = JobOrder.hpsmid, emailstatus = isMailsent == true ? 1 : 0, emaildeliverytime = DateTime.Now, emailremark = "", type = template.type });
                        if (!string.IsNullOrEmpty(JobOrder.customer_rmn))
                        {
                            string smsremark = "";
                            int sentsms = SMS.SendSms(ref smsremark, JobOrder.customer_rmn, message);
                            BLWFMTicket.InsertEmailSmsLog(new wfm_email_sms_log() { joborderid = JobOrder.hpsmid, smsstatus = sentsms, smsdeliverytime = DateTime.Now, smsremark = smsremark, type = template.type });
                        }
                    }
                }

            }
            response.message = "Job order Created Successfully";
            response.status = StatusCodes.OK.ToString();

            return response;
        }

        private bool CreateHPSMTicket(CreateJob objobRequest, SlotRequest objslotRequest, ref string strMessage, ref string strStatus, ref int ticket_tran_id, string jobOrderId = "", string parentJobOrderId = "", string serviceType = "", string service = "")
        {
            bool blnValue = false;

            try
            {
                Models.WFM.Task objTask = new Models.WFM.Task();
                objTask.hpsmid = jobOrderId;
                if (parentJobOrderId != "")
                {
                    objTask.parent_hpsmid = parentJobOrderId;
                }
                if (serviceType != "")
                {
                    objTask.action = serviceType;
                }
                if (service != "")
                {
                    objTask.facility = service;
                }
                objTask.hpsmstatus = "Assigned";
                objTask.status = "Assigned";
                objTask.isresolve = "0";

                if (!string.IsNullOrEmpty(objobRequest.latitude) && !string.IsNullOrEmpty(objobRequest.longitude))
                //if (objobRequest.latitude != "" && objobRequest.longitude != "")
                {
                    objTask.latitude = Convert.ToString(objobRequest.latitude);
                    objTask.longitude = Convert.ToString(objobRequest.longitude);
                }
                else
                {
                    objTask.latitude = Convert.ToString(objslotRequest.latitude);
                    objTask.longitude = Convert.ToString(objslotRequest.longitude);
                }

                objTask.comment_ = "Assignment";
                objTask.tasktype = objslotRequest.order_type;
                objTask.taskcategory = objobRequest.task_category;
                objTask.tasksubcategory = objslotRequest.service_type;
                objTask.created_on = DateTime.Now;
                objTask.customer_preferred_time = objslotRequest.appointment_date;
                objTask.ticket_type = "";
                objTask.ticket_source_id = 1;
                objTask.society_name = "";
                objTask.customer_id = objobRequest.customerid;
                //objTask.customer_add = objobRequest.installation_address;
                objTask.addressid = objobRequest.address_id;
                objTask.addressline1 = objobRequest.address_line1;
                objTask.addressline2 = objobRequest.address_line2;
                objTask.addressline3 = objobRequest.address_line3;
                objTask.addresstype = objobRequest.address_type;
                objTask.city = objobRequest.city;
                objTask.pincode = objobRequest.pinCode;
                objTask.stateorprovince = objobRequest.state_Province;
                objTask.customer_rmn = objobRequest.primary_contact;
                objTask.secondary_contact = objobRequest.secondary_contact;
                objTask.bookingid = objobRequest.bookingid;
                objTask.atomic_id = objobRequest.atomicid;
                objTask.atomicidclose = objobRequest.atomicidClose;
                objTask.access_type = objobRequest.access_type;
                objTask.package_name = objobRequest.package_name;
                objTask.nap_port = objobRequest.nap_port;
                objTask.node = objobRequest.node;
                objTask.cpe_type = objobRequest.cpe_type;
                objTask.cpe_portno = objobRequest.cpe_portno;
                objTask.cpe_brand = objobRequest.cpe_brand;
                objTask.cpe_model = objobRequest.cpe_model;
                objTask.cpe_serialno = objobRequest.cpe_serialno;
                // objTask.facility = objobRequest.facility;

                objTask.email_id = objobRequest.email;
                objTask.local_convergence_point = objobRequest.local_convergence_point;
                objTask.current_cpesn = objobRequest.current_cpesn;
                objTask.listextension_boxsn = objobRequest.listextension_boxsn;
                objTask.comment_ = objobRequest.comment;
                objTask.site_type = objobRequest.site_type;
                objTask.subscriber_name = objobRequest.subscriber_name;

                //objTask.jo_category = objobRequest.jo_category;
                objTask.account_number = objobRequest.account_number;
                objTask.reference_id = objobRequest.reference_id;
                objTask.cpe_mac_address = objobRequest.cpe_mac_address;
                objTask.cpe_item_code = objobRequest.cpe_item_code;
                objTask.jo_category = objslotRequest.jo_category;


                if (!BLWFMTicket.InsertHpsm_TicketMasterData(objTask))
                {
                    strMessage = "Unable save to new job order!!";
                    strStatus = StatusCodes.INVALID_REQUEST.ToString();
                    return blnValue;
                }
                ticket_tran_id = objTask.hpsm_ticket_id;
                blnValue = true;
            }
            catch (Exception ex)
            {
                strMessage = "Error While Processing  Request.";
                strStatus = StatusCodes.INVALID_REQUEST.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(objobRequest);
                logHelper.ApiLogWriter("CreateHPSMTicket()", "TicketController", request, ex);
            }
            return blnValue;
        }

        private bool CreateHPSMTicketNew(CreateJobNew objobRequest, SlotRequest objslotRequest, ref string strMessage, ref string strStatus, ref int ticket_tran_id, string jobOrderId = "", string parentJobOrderId = "", string serviceType = "", string service = "")
        {
            bool blnValue = false;

            try
            {
                Models.WFM.Task objTask = new Models.WFM.Task();
                objTask.hpsmid = jobOrderId;
                if (parentJobOrderId != "")
                {
                    objTask.parent_hpsmid = parentJobOrderId;
                }
                if (serviceType != "")
                {
                    objTask.action = serviceType;
                }
                if (service != "")
                {
                    objTask.facility = service;
                }
                objTask.hpsmstatus = "Assigned";
                objTask.status = "Assigned";
                objTask.isresolve = "0";

                if (!string.IsNullOrEmpty(objobRequest.latitude) && !string.IsNullOrEmpty(objobRequest.longitude))
                //if (objobRequest.latitude != "" && objobRequest.longitude != "")
                {
                    objTask.latitude = Convert.ToString(objobRequest.latitude);
                    objTask.longitude = Convert.ToString(objobRequest.longitude);
                }
                else
                {
                    objTask.latitude = Convert.ToString(objslotRequest.latitude);
                    objTask.longitude = Convert.ToString(objslotRequest.longitude);
                }

                objTask.comment_ = "Assignment";
                objTask.tasktype = objslotRequest.order_type;
                objTask.taskcategory = objobRequest.task_category;
                objTask.tasksubcategory = objslotRequest.service_type;
                objTask.created_on = DateTime.Now;
                objTask.customer_preferred_time = objslotRequest.appointment_date;
                objTask.ticket_type = "";
                objTask.ticket_source_id = 1;
                objTask.society_name = "";
                objTask.customer_id = objobRequest.customerid;
                objTask.customer_add = objobRequest.installation_address;
                objTask.addressid = objobRequest.address_id;
                objTask.addressline1 = objobRequest.address_line1;
                objTask.addressline2 = objobRequest.address_line2;
                objTask.addressline3 = objobRequest.address_line3;
                objTask.addressline4 = objobRequest.address_line4;
                objTask.addressline5 = objobRequest.address_line5;
                objTask.addresstype = objobRequest.address_type;
                objTask.city = objobRequest.city;
                objTask.pincode = objobRequest.pinCode;
                objTask.stateorprovince = objobRequest.state_Province;
                objTask.customer_rmn = objobRequest.primary_contact;
                objTask.secondary_contact = objobRequest.secondary_contact;
                objTask.bookingid = objobRequest.bookingid;
                objTask.atomic_id = objobRequest.atomicid;
                objTask.atomicidclose = objobRequest.atomicidClose;
                objTask.access_type = objobRequest.access_type;
                objTask.package_name = objobRequest.package_name;
                objTask.nap_port = objobRequest.nap_port;
                objTask.node = objobRequest.node;
                objTask.cpe_type = objobRequest.cpe_type;
                objTask.cpe_portno = objobRequest.cpe_portno;
                objTask.cpe_brand = objobRequest.cpe_brand;
                objTask.cpe_model = objobRequest.cpe_model;
                objTask.cpe_serialno = objobRequest.cpe_serialno;
                // objTask.facility = objobRequest.facility;

                objTask.email_id = objobRequest.email;
                objTask.local_convergence_point = objobRequest.local_convergence_point;
                objTask.current_cpesn = objobRequest.current_cpesn;
                objTask.listextension_boxsn = objobRequest.listextension_boxsn;
                objTask.comment_ = objobRequest.comment;
                objTask.site_type = objobRequest.site_type;
                objTask.subscriber_name = objobRequest.subscriber_name;

                objTask.jo_category = objslotRequest.jo_category;
                objTask.account_number = objobRequest.account_number;
                objTask.reference_id = objobRequest.reference_id;
                objTask.cpe_mac_address = objobRequest.cpe_mac_address;
                objTask.cpe_item_code = objobRequest.cpe_item_code;


                objTask.ip = objobRequest.ip;
                objTask.gw = objobRequest.gw;
                objTask.sm = objobRequest.sm;
                objTask.dns = objobRequest.dns;
                objTask.adns = objobRequest.adns;
                objTask.priority = objobRequest.priority;
                objTask.payment_type = objobRequest.paymentType;
                if ((!(string.IsNullOrEmpty(objobRequest.paymentType) )) && (objobRequest.paymentType).ToUpper()=="COD")
                {
                    objTask.payment_status = "Pending";
                }
                objTask.total_amount= objobRequest.paymentTotal;


                if (!BLWFMTicket.InsertHpsm_TicketMasterData(objTask))
                {
                    strMessage = "Unable save to new job order!!";
                    strStatus = StatusCodes.INVALID_REQUEST.ToString();
                    return blnValue;
                }
                ticket_tran_id = objTask.hpsm_ticket_id;
                blnValue = true;
            }
            catch (Exception ex)
            {
                strMessage = "Error While Processing  Request.";
                strStatus = StatusCodes.INVALID_REQUEST.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(objobRequest);
                logHelper.ApiLogWriter("CreateHPSMTicket()", "TicketController", request, ex);
            }
            return blnValue;
        }
        private bool CreateRouteIssue(SlotRequest objslotRequest, ref Route_Issue route_issue, ref string strMessage, ref string strStatus, int ticket_tran_id, ref int route_issue_id)
        {
            bool blnValue = false;
            try
            {
                //List<Issue_Type_Master> objIssueType = BLRoute.GetIssueType();
                //var issue_type = objIssueType.Where(p => p.type_code == objDataIn.issue_type).FirstOrDefault(); //New sale

                //List<Issue_Sub_Type_Master> objIssueSubType = BLRoute.GetIssueSubType();
                //var issue_sub_type = objIssueSubType.Where(p => p.sub_type_code == objDataIn.issue_sub_type).FirstOrDefault();// NS_FTTX_GPON

                //List<Issue_Category_Master> objCategory = BLRoute.GetIssueCategory();
                //var issue_category = objCategory.Where(p => p.category_code == objDataIn.issue_category).FirstOrDefault(); //Normal

                var issue_type = BLWFMTicket.Getservicetype(objslotRequest.service_type); //New sale
                Route_Issue objRI = new Route_Issue();
                objRI.issue_id = 0;
                //objRI.issue_type_id = issue_type.oid;

                objRI.issue_type_id = 0;

                objRI.latitude = objslotRequest.latitude == null ? 0 : objslotRequest.latitude; //objRouteIssueVM.routeIssueInput.latitude;
                objRI.longitude = objslotRequest.longitude == null ? 0 : objslotRequest.longitude;
                //objRI.customer_id = objDataIn.CustomerCount;
                objRI.issue_remark = "";// objslotRequest.remark;
                objRI.hpsm_ticketid = ticket_tran_id;

                //need to look this after fine tune 
                objRI.route_assignment_id = 0;
                objRI.user_id = objslotRequest.managerid;
                objRI.manager_id = objslotRequest.managerid;
                objRI.status = "UnAssigned";
                objRI.user_remark = "";// objRI.issue_remark;
                objRI.modified_on = DateTime.Now;
                objRI.manager_remark = "";
                objRI.mobile_time = DateTime.Now;
                objRI.circle_id = 0;

                if (!BLWFMTicket.SaveRouteIssue(objRI))
                {
                    strMessage = "Unable to save route issues!";
                    strStatus = StatusCodes.INVALID_REQUEST.ToString();
                    return blnValue;
                }
                route_issue = objRI;
                route_issue_id = route_issue.issue_id;
                blnValue = true;
            }
            catch (Exception ex)
            {
                strMessage = "Error While Processing  Request.";
                strStatus = StatusCodes.INVALID_REQUEST.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(objslotRequest);
                logHelper.ApiLogWriter("CreateRouteIssue()", "TicketController", request, ex);
            }
            return blnValue;
        }
        private bool CreatePaymentDetails(CreateJobNew objobRequest, ref string strMessage, ref string strStatus, int ticket_tran_id)
        {
            bool blnValue = false;
            try
            {
              List<Payment_Details> payment_Details= new List<Payment_Details>();
                if (objobRequest.paymentSplitup!=null)
                {
                    foreach (var item in objobRequest.paymentSplitup)
                    {
                        Payment_Details payment_Detail = new Payment_Details();
                        payment_Detail.hpsm_ticket_id = ticket_tran_id;
                        payment_Detail.itemdesc = item.itemdesc;
                        payment_Detail.itemamount = item.itemamount;
                        payment_Detail.job_id = objobRequest.joborderid;
                        if (!BLWFMTicket.SavePaymentDetails(payment_Detail))
                        {
                            strMessage = "Unable to save payment Details!";
                            strStatus = StatusCodes.INVALID_REQUEST.ToString();
                            return blnValue;
                        }
                    }
                }
                                       
                blnValue = true;
            }
            catch (Exception ex)
            {
                strMessage = "Error While Processing  Request.";
                strStatus = StatusCodes.INVALID_REQUEST.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(objobRequest);
                logHelper.ApiLogWriter("CreatePaymentDetails()", "TicketController", request, ex);
            }
            return blnValue;
        }
        private bool SaveRouteIssueStatus(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            VW_Route_Issue objRouteIssue = new VW_Route_Issue();
            bool save = false;
            viewRouteIssueApprove.checkinRadius = 5000;// Convert.ToInt32(ApplicationSettings.DefaultTaskCheckinRadius);
            if (viewRouteIssueApprove.status == "Assigned" || viewRouteIssueApprove.status == "UnAssigned")
            {
                List<Vw_Hpsm_Ticket_Status> hpsmTicketList = new List<Vw_Hpsm_Ticket_Status>();
                save = BLWFMTicket.AssignRouteIssue(viewRouteIssueApprove, out hpsmTicketList);
                //string IsHPSMCall = System.Configuration.ConfigurationManager.AppSettings["HPSMCall"].ToString();
                //if (!string.IsNullOrEmpty(IsHPSMCall))
                //{
                //    //Durgesh unnecessary calls to api 16.9.2021
                //    //if (IsHPSMCall == "true")
                //    //{

                //    //    for (int index = 0; index < hpsmTicketList.Count; index++)
                //    //    {
                //    //        HPSMHelper hpsmHelper = new HPSMHelper(hpsmTicketList[index]);
                //    //    }
                //    //}
                //}
            }
            return save;
        }

        [HttpGet]
        [Route("appointmentDetails")]
        [ValidateModel]
        public WFMApiResponse<dynamic> getAppointmentDetails(AppointmentDetail obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(obRequest.referenceid);
                    if (SlotRequest == null)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Reference id not found";
                        return response;
                    }
                    var result = BLWFMTicket.GetAppointmentDetail(obRequest.referenceid);
                    if (result != null)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.results = result;
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.message = "No record found";
                        return response;
                    }

                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                    logHelper.ApiLogWriter("getAppointmentDetails()", "TicketController", request, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }
            //else
            //{
            //    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            //    response.message = "Invalid format.";
            //}
            return response;
        }



        //Assign ticket
        [HttpPost]
        [Route("createJobOrderTicket")]
        [ValidateModel]
        public WFMApiResponse<dynamic> createJobOrderTicket(CreateJob obRequest)
        {
            var response = new WFMApiResponse<dynamic>();

            try
            {
                if (ModelState.IsValid)
                {
                    // check job id already created or not
                    var isjobId = BLWFMTicket.isJobIdExist(obRequest.joborderid);
                    if (isjobId)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "joborderid " + obRequest.joborderid + " already created.";
                        return response;
                    }

                    var isbookingid = BLWFMTicket.isbookingIdExist(obRequest.bookingid);
                    if (isbookingid)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "bookingid " + obRequest.bookingid + " already created.";
                        return response;
                    }

                    //check booking id available or not
                    SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(obRequest.bookingid);
                    if (Request == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "booking id not found";
                        return response;
                    }
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(Request.referenceid);
                    if (SlotRequest == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Reference id not found";
                        return response;
                    }

                    //validate date
                    if (SlotRequest.appointment_date < DateTime.Now.Date)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "appointment date is less than current date.";
                        return response;
                    }



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    string strMessage = "";
                    string strStatus = "";
                    int ticket_tran_id = 0;
                    Route_Issue objRouteIssue = new Route_Issue();
                    if (!CreateHPSMTicket(obRequest, SlotRequest, ref strMessage, ref strStatus, ref ticket_tran_id))
                    {
                        response.message = strMessage + " Ticket:" + obRequest.joborderid;
                        response.status = strStatus;
                        Exception ex = new Exception(response.status + ":" + response.message);
                        //ErrorLogHelper logHelper = new ErrorLogHelper();
                        //logHelper.ApiLogWriter("CreateHPSMTicketfor()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                        return response;
                    }
                    int route_issue_id = 0;
                    if (!CreateRouteIssue(SlotRequest, ref objRouteIssue, ref strMessage, ref strStatus, ticket_tran_id, ref route_issue_id))
                    {
                        response.message = strMessage + " Ticket:" + obRequest.joborderid;
                        response.status = strStatus;
                        Exception ex = new Exception(response.status + ":" + response.message);
                        //ErrorLogHelper logHelper = new ErrorLogHelper();
                        //logHelper.ApiLogWriter("CreateRouteIssueforNMS()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                        return response;
                    }



                    ViewManagerRouteIssueApprove objMRIA = new ViewManagerRouteIssueApprove();
                    // objMRIA.lstUserDetail = BLUser.GetSubordinateDetails(intManagerId, "FRT").ToList();

                    //objMRIA.issueId = objRouteIssue.issue_id;
                    //objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                    //objMRIA.user_id = SlotRequest.managerid;
                    //objMRIA.frtUserId = SlotRequest.managerid;
                    //objMRIA.status = "UnAssigned";
                    //objMRIA.remarks = "Assigned Ticket To Manager";
                    //objMRIA.assignedDate = DateTime.Now;


                    var FEList = BLWFMTicket.GetFEList(SlotRequest.managerid);

                    if (FEList.Count > 0)
                    {
                        objMRIA.issueId = objRouteIssue.issue_id;
                        objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                        objMRIA.user_id = SlotRequest.managerid;
                        objMRIA.frtUserId = Convert.ToInt32(FEList.FirstOrDefault().UserId);
                        objMRIA.status = "Assigned";
                        objMRIA.remarks = "Automatically Assigned Ticket";
                        objMRIA.assignedDate = DateTime.Now;
                    }
                    else
                    {
                        objMRIA.issueId = objRouteIssue.issue_id;
                        objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                        objMRIA.user_id = SlotRequest.managerid;
                        objMRIA.frtUserId = SlotRequest.managerid;
                        objMRIA.status = "UnAssigned";
                        objMRIA.remarks = "Assigned Ticket To Manager";
                        objMRIA.assignedDate = DateTime.Now;
                    }

                    if (!SaveRouteIssueStatusTKT(objMRIA))
                    {
                        response.message = "Unable to Assign ticket to frt. Manager needs to assign ticket " + obRequest.joborderid + " to FE manually";
                        response.status = strStatus;
                        Exception ex = new Exception(response.status + ":" + response.message);
                        //ErrorLogHelper logHelper = new ErrorLogHelper();
                        //logHelper.ApiLogWriter("CreateRouteIssueforNMS()", "TicketController", JsonConvert.SerializeObject(objDataIn), ex);
                        return response;

                    }
                    else
                    {
                        try
                        {
                            //NotificationHelper notificatonHelper = new NotificationHelper();
                            //var ids = objMRIA.issuesId.Split(',');
                            //for (int i = 0; i < ids.Length; i++)
                            //{
                            //    string strNotMess = "Task/Ticket id:" + objDataIn.ticket_id + " has been assigned.";
                            //    notificatonHelper.sendNotification(objMRIA.frtUserId, Convert.ToInt32(ids[i]), strNotMess, "Task/Ticket Assigned");
                            //}
                        }
                        catch (Exception ec)
                        {
                            //ErrorLogHelper logHelper = new ErrorLogHelper();
                            //logHelper.ApiLogWriter("SaveRouteIssueStatus() + notificatonHelper", "Ticket Controller", JsonConvert.SerializeObject(objDataIn), ec);
                        }
                    }


                    response.message = "Job order Created Successfully";
                    response.status = StatusCodes.OK.ToString();

                }
            }
            catch (Exception ex)
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.message = "Error While Processing  Request. " + ex.Message + "-->" + ex.InnerException;
            }
            return response;
        }

        private bool SaveRouteIssueStatusTKT(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            VW_Route_Issue objRouteIssue = new VW_Route_Issue();
            bool save = false;
            viewRouteIssueApprove.checkinRadius = 5000;// Convert.ToInt32(ApplicationSettings.DefaultTaskCheckinRadius);
            if (viewRouteIssueApprove.status == "Assigned" || viewRouteIssueApprove.status == "UnAssigned")
            {
                List<Vw_Hpsm_Ticket_Status> hpsmTicketList = new List<Vw_Hpsm_Ticket_Status>();
                save = BLWFMTicket.AssignRouteIssueTKT(viewRouteIssueApprove, out hpsmTicketList);
                //string IsHPSMCall = System.Configuration.ConfigurationManager.AppSettings["HPSMCall"].ToString();
                //if (!string.IsNullOrEmpty(IsHPSMCall))
                //{
                //    //Durgesh unnecessary calls to api 16.9.2021
                //    //if (IsHPSMCall == "true")
                //    //{

                //    //    for (int index = 0; index < hpsmTicketList.Count; index++)
                //    //    {
                //    //        HPSMHelper hpsmHelper = new HPSMHelper(hpsmTicketList[index]);
                //    //    }
                //    //}
                //}
            }
            return save;
        }


        [HttpPost]
        [Route("cancelAppointmentSlot")]
        [ValidateModel]
        public WFMApiResponse<dynamic> cancelAppointmentSlot(SlotConfirmation obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(obRequest.referenceid);
                    if (SlotRequest == null)
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.message = "Reference id not found";
                        return response;
                    }

                    if (SlotRequest.appointment_date < DateTime.Now.Date)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "appointment date is less than current date.";
                        return response;
                    }

                    var result = BLWFMTicket.GetSlotConBySlotRefId(obRequest.slotid, obRequest.referenceid);
                    if (result == null)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "No record found";
                        return response;
                    }
                    var cnfBookin = BLWFMTicket.isbookingIdExist(result.bookingid);

                    if (cnfBookin)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Can not cancel Appointment because job order is created";
                        return response;
                    }
                    var cancle = BLWFMTicket.cancleAppointmentSlot(result.bookingid);
                    if (cancle == -1)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.message = "Appointment cancelled successfully";
                        response.results = null;
                        return response;
                    }


                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                    logHelper.ApiLogWriter("cancelAppointmentSlot()", "TicketController", request, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }
            //else
            //{
            //    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            //    response.message = "Invalid format.";
            //}
            return response;
        }

        [HttpPost]
        [Route("cancelJobOrder")]
        [ValidateModel]
        public WFMApiResponse<dynamic> cancelJobOrder(CancelJobOrder obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    //check slot available or not by refid
                    Models.WFM.Task JobOrder = BLWFMTicket.GetJobDetailByJobOrderId(obRequest.joborderid);
                    if (JobOrder == null)
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.message = "JobOrderId id not found";
                        return response;
                    }

                    //if (JobOrder.customer_preferred_time < DateTime.Now.Date)
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = "appointment date is less than current date.";
                    //    return response;
                    //}

                    var routeIssues = BLWFMTicket.GetRoute_Issue(JobOrder.hpsm_ticket_id);
                    if (routeIssues == null)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.message = "not record found in job order assignment";
                        return response;
                    }
                    if (routeIssues.status != "UnAssigned")
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.message = "Can not cancel job order, because job order already cancelled or assigned to EF";
                        return response;
                    }
                    var cancle = BLWFMTicket.cancleJobOrder(JobOrder.hpsm_ticket_id);
                    if (cancle == -1)
                    {
                        var AppointmentCancel = BLWFMTicket.cancleAppointmentSlot(JobOrder.bookingid);
                        response.status = StatusCodes.OK.ToString();
                        response.message = "Job Order cancelled successfully";
                        response.results = null;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                    logHelper.ApiLogWriter("cancelJobOrder()", "TicketController", request, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }
            //else
            //{
            //    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            //    response.message = "Invalid format.";
            //}
            return response;
        }

        [HttpPost]
        [Route("rescheduleAppointmentSlot")]
        [ValidateModel]
        public WFMApiResponse<dynamic> rescheduleAppointmentSlot(SlotConfirmation obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(obRequest.referenceid);
                    int slotManagerId = Convert.ToInt32(obRequest.slotid.Split('_')[1]);
                    SlotRequest.managerid = slotManagerId;
                    if (SlotRequest == null)
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.message = "Reference id not found";
                        return response;
                    }

                    var result = BLWFMTicket.GetSlotConfirmationByRefId(obRequest.referenceid, 0);
                    if (result == null)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Can not Appointment reschedule slot is not book.";
                        return response;

                    }

                    if (result.slotid == obRequest.slotid)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Can not reschedule  slot time is same.";
                        return response;
                    }
                    var cnfBookin = BLWFMTicket.isbookingIdExist(result.bookingid);

                    if (cnfBookin)
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "Can not reschedule Appointment because job order is created";
                        return response;
                    }
                    //re-shedule appointment
                    DateTime appointment_date;
                    int maste_slot_id;
                    BLWFMTicket.GetValue(obRequest.slotid, out appointment_date, out maste_slot_id);
                    obRequest.appointment_date = appointment_date;
                    obRequest.master_slot_id = maste_slot_id;
                    obRequest.updated_date = DateTime.Now;

                    #region Validate jo type and service
                    string message;
                    string joType;
                    string add_service;
                    if (!ValidateJoServiceType(SlotRequest, out message, out joType, out add_service))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = message;
                        return response;
                    }

                    var JoRoleMapping = BLWFMTicket.GetRoleNameByJoCategory(SlotRequest.jo_category, joType);
                    string fe_type = JoRoleMapping.workflow == "Regular process" ? "regular" : "fastlane";
                    #endregion

                    string femessage;
                    int slotId;
                    List<FEList> AVFEList;
                    if (!GetEFList(SlotRequest, out slotId, out AVFEList, SlotRequest.managerid, out femessage, joType, add_service, fe_type))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = femessage;
                        return response;
                    }


                    #region Check roster

                    //if (AVFEList.Count <= 0)
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = "No FE found.";
                    //    return response;
                    //}


                    //string strTimeSheetError = "";
                    //DateTime dtCurDate = DateTime.Now;
                    //int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                    //AVFEList.ForEach(r =>
                    //{
                    //    string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                    //    bool isTimeSheet = BLWFMTicket.isTimeSheetDefined(r.UserId, DateTime.Now, intDayOfWeek, strTime, out strTimeSheetError);

                    //    if (isTimeSheet)

                    //        r.isRosterAvailable = true;
                    //    else
                    //        r.isRosterAvailable = false;
                    //});

                    //if (AVFEList.Count > 0 && !AVFEList.Any(a => a.isRosterAvailable == true))
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = AVFEList.Count + " FEs found but no roster available";
                    //    return response;
                    //}
                    //var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

                    #endregion

                    #region Check roster

                    //string strTimeSheetError = "";
                    DateTime dtCurDate = DateTime.Now;//
                    int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                    AVFEList.ForEach(r =>
                    {
                        string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                        //bool isTimeSheet = BLWFMTicket.isTimeSheetDefined(r.UserId ?? 0, DateTime.Now, intDayOfWeek, strTime, out strTimeSheetError);
                        var isTimeSheet = BLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, appointment_date, intDayOfWeek, strTime);

                        if (isTimeSheet != null)
                        {
                            r.isRosterAvailable = true;
                            r._start_time = isTimeSheet._start_time;
                            r._end_time = isTimeSheet._end_time;
                        }
                        else
                        {
                            r.isRosterAvailable = false;
                        }
                    });

                    if (AVFEList.Count > 0 && !AVFEList.Any(a => a.isRosterAvailable == true))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = AVFEList.Count + " FEs found but no roster available";
                        return response;
                    }
                    var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

                    #endregion





                    var slot = BLWFMTicket.GetFreeSlot(obRequest.slotid, SlotRequest.managerid);

                    int totalslot = AvailableEF.Count - slot;

                    //set manager userid
                    obRequest.managerid = SlotRequest.managerid;

                    if (totalslot > 0)
                    {
                        int bookingId = BLWFMTicket.UpdateSlotConfirmation(obRequest);
                        if (bookingId > 0)
                        {
                            //update appointment date in slot request
                            BLWFMTicket.updateSlotRequestAppmntDate(obRequest.referenceid, obRequest.appointment_date);
                            BookingResponce resbokking = new BookingResponce() { bookingid = bookingId };
                            response.status = StatusCodes.OK.ToString();
                            response.message = "Appointment reschedule successfully";
                            response.results = resbokking;
                        }
                    }
                    else
                    {
                        response.status = StatusCodes.INVALID_REQUEST.ToString();
                        response.message = "Sorry ! Slot booked";
                    }

                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                    logHelper.ApiLogWriter("rescheduleAppointmentSlot()", "TicketController", request, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }
            //else
            //{
            //    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            //    response.message = "Invalid format.";
            //}
            return response;
        }


        [HttpPost]
        [Route("updateCpeStatus")]
        [ValidateModel]
        public WFMApiResponse<dynamic> updateCpeStatus(dynamic inpuRequest)
        {

            var response = new WFMApiResponse<dynamic>();
            job_order_status jobOrderStatus = new job_order_status();
            if (ModelState.IsValid)
            {
                try
                {
                    string jobId = ((inpuRequest)["jobid"]);
                    string status = ((inpuRequest)["status"]);
                    string message = ((inpuRequest)["message"]);
                    Models.WFM.Task jobOrder = BLWFMTicket.GetTTJobDetailByJobOrderId(jobId);
                    if (jobOrder != null && jobOrder.ticket_source_id != 4)
                    {
                        string atomicOrderId = ((inpuRequest)["atomicOrderId"]);
                        jobOrder = BLWFMTicket.GetJobDetailByJobOrderIdAtomicId(jobId, atomicOrderId);
                    }
                    //Models.WFM.Task jobOrder = BLWFMTicket.GetJobDetailByJobOrderId(jobId);
                    //GetTTJobDetailByJobOrderId
                    int stepOrder = BLWFMTicket.GetStepOrder();

                    if (jobOrder != null)
                    {
                        jobId = jobOrder.hpsmid;
                        int statusRes = 0;
                        if (status.ToUpper() == "SUCCESS")
                        {
                            jobOrderStatus.job_id = jobId;
                            jobOrderStatus.action = "cpe_activate";
                            jobOrderStatus.remarks = message;
                            jobOrderStatus.service_status = "CPE activated";
                            if (stepOrder > 0)
                            {
                                if (jobOrder.facility == "STATIC_IP")
                                {

                                    int staticIPRes = BLWFMTicket.UpdateStaticIPStatus(jobOrder.hpsmid, message);
                                    if (staticIPRes > 0)
                                    {
                                        response.status = StatusCodes.OK.ToString();
                                        response.message = "Static IP status updated successfully, JO:" + jobId;

                                    }
                                    else
                                    {
                                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                        response.message = "Some error in updating Static IP status !!";
                                    }
                                    //  return response;
                                }
                                else
                                {
                                    statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                                    int res = BLWFMTicket.UpdateJobOrderStage(jobId, stepOrder, null);
                                    if (statusRes > 0)
                                    {
                                        response.status = StatusCodes.OK.ToString();
                                        response.message = "CPE status updated successfully, JO:" + jobId;

                                    }
                                    else
                                    {
                                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                        response.message = "Some error in updating CPE status !!";
                                    }
                                }

                            }
                            else
                            {
                                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                response.message = "Step Order Id not found";
                            }

                        }
                        else if (status.ToUpper() == "FAIL")
                        {
                            jobOrderStatus.job_id = jobId;
                            jobOrderStatus.action = "cpe_activation_fail";
                            jobOrderStatus.remarks = message;
                            // jobOrderStatus.service_status = "CPE activation fail";
                            statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                            if (statusRes > 0)
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.message = "CPE status updated successfully ";

                            }
                            else
                            {
                                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                response.message = "Some error in updating CPE status !!";
                            }

                        }
                        else
                        {
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.message = "Invalid status name";
                        }


                    }
                    else
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.message = "JobOrderId id not found";
                    }

                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("updateCpeStatus()", "TicketController", Convert.ToString(inpuRequest), ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }

            return response;
        }


        [HttpPost]
        [Route("updateConnectedDevice")]
        [ValidateModel]
        public WFMApiResponse<dynamic> updateConnectedDevice(Models.WFM.Root root)
        {

            var response = new WFMApiResponse<dynamic>();
            var requestJson = "";
            if (ModelState.IsValid)
            {
                try
                {
                    requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(root);
                    Models.WFM.ConnectedDeviceRequest ConnectedDeviceDetail = BLWFMTicket.ConnectedDeviceDetailByRequestId(root.requestid);
                    if (ConnectedDeviceDetail != null)
                    {
                        int result = BLWFMTicket.updateConnectedDevice(root);
                        if (result > 0)
                        {
                            response.status = StatusCodes.OK.ToString();
                            response.message = "Connected devices updated successfully";
                        }
                        else
                        {
                            response.status = StatusCodes.FAILED.ToString();
                            response.message = "Connected device not updated !!";
                        }
                    }
                    else
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Request Id not found !!";
                    }

                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("updateConnectedDevice()", "TicketController", requestJson, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }

            return response;
        }




        //createJobOrder new
        [HttpPost]
        [Route("createJobOrdernew")]
        [ValidateModel]
        public WFMApiResponse<dynamic> createJobOrdernew(CreateJobNew obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            try
            {
                if (ModelState.IsValid)
                {
                    int jobIdSequence = 1;
                    // check job id already created or not
                    var isjobId = BLWFMTicket.isJobIdExist(obRequest.joborderid);
                    if (isjobId)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "joborderid " + obRequest.joborderid + " already created.";
                        return response;
                    }
                    var JoCategory = BLJobOrder.GetJoCategoryByCode(obRequest.jo_category);
                    if (JoCategory == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Jo Category Code " + obRequest.jo_category + " is not found.";
                        return response;
                    }
                    obRequest.priority = JoCategory.priority;

                    if (obRequest.bookingid == 0)
                    {
                        int managerid = 0;
                        string mgrName = "";
                        string barangay = "";
                        if (string.IsNullOrEmpty(obRequest.latitude))
                        {
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.message = "Latitude can not be null.";
                            return response;
                        }
                        if (string.IsNullOrEmpty(obRequest.longitude))
                        {
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.message = "Longitude can not be null.";
                            return response;
                        }

                        foreach (var service in obRequest.Services)
                        {
                            string parentJobOrderId = obRequest.joborderid;
                            string jobOrderId = obRequest.Services.Count == 1 ? obRequest.joborderid : obRequest.joborderid + "-" + jobIdSequence;

                            obRequest.facility = service.facility;
                            //obRequest.Action = service.action;
                            obRequest.atomicid = service.atomicidActivate;
                            obRequest.atomicidClose = service.atomicidClose;
                            obRequest.nap_port = service.nap_port;
                            obRequest.node = service.node;
                            obRequest.cpe_type = service.cpe_type;
                            obRequest.cpe_portno = service.cpe_portno;
                            obRequest.cpe_brand = service.cpe_brand;
                            obRequest.cpe_model = service.cpe_model;
                            obRequest.cpe_serialno = service.cpe_serialno;
                            obRequest.listextension_boxsn = service.listextension_boxsn;
                            obRequest.cpe_mac_address = service.cpe_mac_address;
                            obRequest.cpe_item_code = service.cpe_item_code;
                            obRequest.package_name = service.package_name;

                            //Address
                            obRequest.address.ForEach(f =>
                            {
                                obRequest.address_line1 = f.address_line1;
                                obRequest.address_line2 = f.address_line2;
                                obRequest.address_line3 = f.address_line3;
                                obRequest.address_line4 = f.address_line4;
                                obRequest.address_line5 = f.address_line5;
                                obRequest.address_type = f.address_type;
                                obRequest.city = f.city;
                                obRequest.pinCode = f.pincode;
                                obRequest.state_Province = f.stateorprovince;
                                obRequest.address_id = f.addressid;

                            });


                            obRequest.ip = service.ip;
                            obRequest.gw = service.gw;
                            obRequest.sm = service.sm;
                            obRequest.dns = service.dns;
                            obRequest.adns = service.adns;

                            if (service.facility == "IPTV" || service.facility == "DEVICE" || service.facility == "IPTV_EXTN" || service.facility == "VAS")
                            {
                                //GET COFG Dispatcher
                                var Manager = BLWFMTicket.GetManagerList(Convert.ToDouble(obRequest.latitude), Convert.ToDouble(obRequest.longitude), "COFG Dispatcher");
                                var ManagerList = Manager.GroupBy(x => x.user_id).OrderBy(o => o.Key).Select(g => new { user_id = g.Key, manager = g.FirstOrDefault() }).ToList();

                                if (ManagerList.Count > 0)
                                {
                                    foreach (var group in ManagerList)
                                    {
                                        managerid = group.user_id ?? 0;
                                        mgrName = group.manager.user_name;
                                        barangay = group.manager.block_name;

                                        if (managerid > 0)
                                            break;
                                    }
                                }

                                if (Manager.Count == 0)
                                {
                                    response.status = StatusCodes.FAILED.ToString();
                                    response.message = "No barangay area found in requested lat lng";
                                    return response;
                                }

                                if (managerid == 0)
                                {
                                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                                    response.message = "No dispatcher found with COFG Dispatcher role in " + barangay;
                                    return response;
                                }

                                //create job order
                                var res = CreateTicketNew(obRequest, new SlotRequest() { managerid = managerid, latitude = Convert.ToDouble(obRequest.latitude), longitude = Convert.ToDouble(obRequest.longitude) }, jobOrderId, parentJobOrderId, service.Action, service.facility);
                                if (res.status != StatusCodes.OK.ToString())
                                {
                                    response.status = res.status;
                                    response.message = res.message;
                                    return response;
                                }
                                response.status = res.status;
                                response.message = string.Concat(res.message, " ,assign to COFG Dispatcher : ", mgrName);

                                return response;

                            }
                            else
                            {
                                // booking is 0 for this type of service

                                response.status = StatusCodes.INVALID_INPUTS.ToString();
                                response.message = "Booking id Can not be 0 for " + obRequest.facility + " type of service";
                                return response;
                            }

                        }
                    }


                    //var isbookingid = BLWFMTicket.isbookingIdExist(obRequest.bookingid);
                    //if (isbookingid)
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = "bookingid " + obRequest.bookingid + " already created.";
                    //    return response;
                    //}

                    //check booking id available or not
                    SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(obRequest.bookingid);
                    if (Request == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "booking id not found";
                        return response;
                    }
                    //check slot available or not by refid
                    SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(Request.referenceid);
                    SlotRequest.managerid = Request.managerid;
                    if (SlotRequest == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Reference id not found";
                        return response;
                    }

                    //validate date
                    //if (SlotRequest.appointment_date < DateTime.Now.Date)
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = "appointment date is less than current date.";
                    //    return response;
                    //}

                    //Address
                    obRequest.address.ForEach(f =>
                    {
                        obRequest.address_line1 = f.address_line1;
                        obRequest.address_line2 = f.address_line2;
                        obRequest.address_line3 = f.address_line3;
                        obRequest.address_line4 = f.address_line4;
                        obRequest.address_line5 = f.address_line5;
                        obRequest.address_type = f.address_type;
                        obRequest.city = f.city;
                        obRequest.pinCode = f.pincode;
                        obRequest.state_Province = f.stateorprovince;
                        obRequest.address_id = f.addressid;

                    });



                    // var serviceType = BLWFMTicket.Getservicetype(SlotRequest.service_type);

                    //if (serviceType == null)
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = "Service type not found";
                    //    return response;
                    //}
                    //if (string.IsNullOrEmpty(serviceType.add_service) && string.IsNullOrEmpty(serviceType.remove_service))
                    //{
                    //    response.status = StatusCodes.FAILED.ToString();
                    //    response.message = string.Format("Add/Remove service facility not map with service type : {0}", SlotRequest.service_type);
                    //    return response;
                    //}

                    #region Create job order for add service/remove service

                    List<AddRemoveservice> AddService = new List<AddRemoveservice>();
                    //if (!string.IsNullOrEmpty(serviceType.add_service))
                    //{
                    //    if (serviceType.add_service.Contains(","))
                    //    {
                    //        serviceType.add_service.Split(',').ToList().ForEach(x =>
                    //        {
                    //            AddService.Add(new AddRemoveservice() { service = x, action = "Add" });
                    //        });
                    //    }
                    //    else
                    //    {
                    //        AddService.Add(new AddRemoveservice() { service = serviceType.add_service, action = "Add" });
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(serviceType.remove_service))
                    //{
                    //    if (serviceType.remove_service.Contains(","))
                    //    {
                    //        serviceType.remove_service.Split(',').ToList().ForEach(x =>
                    //        {
                    //            AddService.Add(new AddRemoveservice() { service = x, action = "Remove" });
                    //        });
                    //    }
                    //    else
                    //    {
                    //        AddService.Add(new AddRemoveservice() { service = serviceType.add_service, action = "Remove" });
                    //    }
                    //}
                    //create job order for add service
                    //foreach (var service in AddService)
                    //{
                    //    //create job order id
                    //    string parentJobOrderId = obRequest.joborderid;
                    //    string jobOrderId = AddService.Count == 1 ? obRequest.joborderid : obRequest.joborderid + "-" + jobIdSequence;

                    //    //create job order
                    //    var res = CreateTicket(obRequest, SlotRequest, jobOrderId, parentJobOrderId, service.action, service.service);
                    //    if (res.status != StatusCodes.OK.ToString())
                    //    {
                    //        response.status = res.status;
                    //        response.message = res.message;
                    //        return response;
                    //    }
                    //    response.status = res.status;
                    //    //response.message = res.message + " ,assign to contractor : " + managerName;
                    //    response.message = string.Concat(res.message, " ,assign to contractor : ", managerName);
                    //    jobIdSequence++;
                    //}

                    //Rule changes
                    foreach (var service in obRequest.Services)
                    {
                        string parentJobOrderId = obRequest.joborderid;
                        string jobOrderId = obRequest.Services.Count == 1 ? obRequest.joborderid : obRequest.joborderid + "-" + jobIdSequence;

                        obRequest.facility = service.facility;
                        //obRequest.Action = service.action;
                        obRequest.atomicid = service.atomicidActivate;
                        obRequest.atomicidClose = service.atomicidClose;
                        obRequest.nap_port = service.nap_port;
                        obRequest.node = service.node;
                        obRequest.cpe_type = service.cpe_type;
                        obRequest.cpe_portno = service.cpe_portno;
                        obRequest.cpe_brand = service.cpe_brand;
                        obRequest.cpe_model = service.cpe_model;
                        obRequest.cpe_serialno = service.cpe_serialno;
                        obRequest.listextension_boxsn = service.listextension_boxsn;
                        obRequest.cpe_mac_address = service.cpe_mac_address;
                        obRequest.cpe_item_code = service.cpe_item_code;
                        obRequest.package_name = service.package_name;


                        obRequest.ip = service.ip;
                        obRequest.gw = service.gw;
                        obRequest.sm = service.sm;
                        obRequest.dns = service.dns;
                        obRequest.adns = service.adns;


                        if (service.facility == "IPTV" || service.facility == "DEVICE" || service.facility == "IPTV_EXTN" || service.facility == "VAS")
                        {
                            //GET COFG Dispatcher
                            var Manager = BLWFMTicket.GetManagerList(Convert.ToDouble(SlotRequest.latitude), Convert.ToDouble(SlotRequest.longitude), "COFG Dispatcher");
                            var ManagerList = Manager.GroupBy(x => x.user_id).OrderBy(o => o.Key).Select(g => new { user_id = g.Key }).ToList();

                            if (ManagerList.Count > 0)
                            {
                                foreach (var group in ManagerList)
                                {
                                    SlotRequest.managerid = group.user_id ?? 0;

                                    if (group.user_id > 0)
                                        break;
                                }
                            }
                            else
                            {
                                ErrorLogHelper logHelper = new ErrorLogHelper();
                                var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                                logHelper.ApiLogWriter("createJobOrdernew()", "TicketController", "Manger not found for IPTV/ Device/ IPTV_EXTN/ VAS :- " + request, null);

                            }
                        }
                        //string managerName = BLWFMTicket.GetUserNameById(SlotRequest.managerid);
                        var manager = BLWFMTicket.GetUserById(SlotRequest.managerid);
                        string ManagerName = string.Empty;
                        string ManagerEmail = string.Empty;
                        if (manager != null)
                        {
                            ManagerName = manager.user_name;
                            ManagerEmail = manager.user_email;
                        }

                        //create job order
                        var res = CreateTicketNew(obRequest, SlotRequest, jobOrderId, parentJobOrderId, service.Action, service.facility);
                        if (res.status != StatusCodes.OK.ToString())
                        {
                            response.status = res.status;
                            response.message = res.message;

                            var isSentMail = Convert.ToBoolean(ConfigurationManager.AppSettings["isSentMail"]);
                            if (isSentMail)
                            {
                                var JoRoleMapping = BLWFMTicket.GetRoleNameByJoCategory(SlotRequest.jo_category, SlotRequest.order_type);
                                if (JoRoleMapping != null)
                                {
                                    if (JoRoleMapping.workflow.ToLower() == "Fast Lane".ToLower())
                                    {
                                        var message = "Dear user, Fastlane JO id " + jobOrderId + " is pending for dispatch";
                                        string subject = "Notification regarding Fast Lane job order";
                                        if (!string.IsNullOrEmpty(ManagerEmail))
                                        {
                                            var isMailsent = Email.sendMail("", ManagerEmail, subject, message);
                                            BLUser.UpdateNotificationForFastLane("FastLane", jobOrderId, isMailsent == true ? "1" : "0");
                                        }
                                    }
                                }
                            }
                            return response;
                        }
                        response.status = res.status;
                        //response.message = res.message + " ,assign to contractor : " + managerName;
                        response.message = string.Concat(res.message, " ,assign to dispatcher : ", ManagerName);
                        jobIdSequence++;
                    }



                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                logHelper.ApiLogWriter("createJobOrdernew()", "TicketController", request, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.message = "Error While Processing  Request.";
            }
            return response;
        }

        //createTTJobOrder
        [HttpPost]
        [Route("createTTJobOrder")]
        [ValidateModel]
        public WFMApiResponse<dynamic> createTTJobOrder(CreateTTJob obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            try
            {
                if (ModelState.IsValid)
                {
                    // check job id already created or not
                    int jobIdSequence = 0;
                    string parentjobid = obRequest.tt_id;
                    var isjobId = BLWFMTicket.isJobIdExist(obRequest.tt_id);
                    if (isjobId)
                    {
                        jobIdSequence = BLWFMTicket.isJobIdExistCount(obRequest.tt_id);
                        obRequest.tt_id = obRequest.tt_id + "_" + jobIdSequence;
                        //response.status = StatusCodes.FAILED.ToString();
                        //response.message = "joborderid " + obRequest.tt_id + " already created.";
                        //return response;
                    }
                    var TTCategory = BLJobOrder.GetttcatgeoryByName(obRequest.tt_category);
                    if (TTCategory == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "TT Category: " + obRequest.tt_category + " doesn't exist.";
                        return response;
                    }
                    var TTTpye = BLJobOrder.GetttTypeByName(obRequest.tt_type);
                    if (TTTpye == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "TT Type: " + obRequest.tt_type + " doesn't exist.";
                        return response;
                    }
                    var TTCustomerSegment = BLJobOrder.GettCustomerSegmentByName(obRequest.customer_segment);
                    if (TTCustomerSegment == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "TT Customer Segment: " + obRequest.customer_segment + " doesn't exist.";
                        return response;
                    }
                    var TTCustomerCategory = BLJobOrder.GettCustomerCategoryByName(obRequest.customer_category);
                    if (TTCustomerCategory == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "TT Customer Category: " + obRequest.customer_category + " doesn't exist.";
                        return response;
                    }
                    var roleName = "";
                    if (obRequest.customer_category == "CORPORATE")
                    {
                        roleName = "In House Supervisor";
                    }
                    else
                    {
                        roleName = "MSP Supervisor";
                    }
                    var ManagerList = new List<ManagerList>();
                    string blockName = BLWFMTicket.GetBlockName(Convert.ToDouble(obRequest.latitude), Convert.ToDouble(obRequest.longitude));
                    if (String.IsNullOrEmpty(blockName))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "No barangay area found in requested lat lng";
                        return response;
                    }


                    ManagerList = BLWFMTicket.GetManagerList(Convert.ToDouble(obRequest.latitude), Convert.ToDouble(obRequest.longitude), roleName);
                    if (ManagerList.Count == 0)
                    {
                        roleName = roleName == "In House Supervisor" ? "MSP Supervisor" : "In House Supervisor";
                        ManagerList = BLWFMTicket.GetManagerList(Convert.ToDouble(obRequest.latitude), Convert.ToDouble(obRequest.longitude), roleName);
                    }
                    if (ManagerList.Count == 0)
                    {

                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Supervisor not found for " + blockName;
                        return response;
                    }
                    if (ManagerList.Count == 1 && ManagerList.FirstOrDefault().user_id == null)
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.message = "Supervisor not found for " + ManagerList.FirstOrDefault().block_name;
                        return response;
                    }
                    List<ManagerList> mgl = new List<ManagerList>();
                    foreach (var item in ManagerList)
                    {
                        var supervisiororFrtCountDetail = BLWFMTicket.GetSupervisiorCountDetail(item.user_id);
                        item.dispatchercount = supervisiororFrtCountDetail.Count;
                        mgl.Add(item);
                    }

                    mgl = mgl.OrderBy(j => j.dispatchercount).ToList();
                    foreach (var item in mgl)
                    {
                        var ManagerName = item.user_name;
                        var ManagerId = item.user_id ?? 0;


                        //create job order id
                        string parentJobOrderId = parentjobid;

                        //create job order
                        var res = CreateTTTicket(obRequest, obRequest.tt_id, parentJobOrderId, ManagerId, ManagerName, roleName);
                        if (res.status != StatusCodes.OK.ToString())
                        {
                            response.status = res.status;
                            response.message = res.message;
                            // return response;
                        }
                        else
                        {
                            response.status = res.status;
                            //response.message = res.message + " ,assign to contractor : " + managerName;
                            response.message = string.Concat(res.message, " ", "");
                            break;
                        }
                        //      jobIdSequence++;
                        //     }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                logHelper.ApiLogWriter("createTTJobOrder()", "TicketController", request, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.message = "Error While Processing  Request.";
            }
            return response;
        }
        public WFMApiResponse<dynamic> CreateTTTicket(CreateTTJob obRequest, string jobOrderId, string parentJobOrderId, int managerId, string managerName, string managerRole)
        {
            var response = new WFMApiResponse<dynamic>();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string strMessage = "";
            string strStatus = "";
            int ticket_tran_id = 0;

            Route_Issue objRouteIssue = new Route_Issue();
            if (!CreateHPSMTTTicket(obRequest, ref strMessage, ref strStatus, ref ticket_tran_id, jobOrderId, parentJobOrderId))
            {
                response.message = strMessage + " Ticket:" + obRequest.tt_id;
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                return response;
            }
            int route_issue_id = 0;
            if (!CreateTTRouteIssue(obRequest, ref objRouteIssue, ref strMessage, ref strStatus, ticket_tran_id, ref route_issue_id, managerId))
            {
                response.message = strMessage + " Ticket:" + obRequest.tt_id;
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                return response;
            }
            string message = "Job order created successfully and assign to: ";
            ViewManagerRouteIssueApprove objMRIA = new ViewManagerRouteIssueApprove();
            string logMessage = "";
            ErrorLogHelper logHelper = new ErrorLogHelper();
            if (managerId > 0)
            {

                //var FEListbymanager = BLWFMTicket.GetFEList(managerId);
                string FErole = "";
                if (managerRole == "MSP Supervisor")
                {
                    FErole = "MSP Full Vehicle Team";
                }
                else
                {
                    FErole = "In House Full Vehicle Team";
                }
                //var FEList = FEListbymanager.Where(f => f.role_name == FErole).ToList();
                var FEList = BLWFMTicket.GetFEListbylocation(managerId, Convert.ToDouble(obRequest.latitude), Convert.ToDouble(obRequest.longitude), FErole);
                if (FEList.Count == 0)
                {
                    response.status = StatusCodes.OK.ToString();
                    response.message = "FEs not found , ticket assign to: " + managerName;
                    logMessage = "Ticket-" + obRequest.tt_id + ": " + " FRT not found , ticket assign to: " + managerName;
                    logHelper.ApiLogWriter("createTTJobOrder()", "TicketController", logMessage, null);
                    return response;
                }
                #region Check roster

                //string strTimeSheetError = "";
                DateTime dtCurDate = DateTime.Now;//
                int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
                FEList.ForEach(r =>
                {
                    string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                    //bool isTimeSheet = BLWFMTicket.isTimeSheetDefined(r.UserId ?? 0, DateTime.Now, intDayOfWeek, strTime, out strTimeSheetError);
                    var isTimeSheet = BLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, DateTime.Now, intDayOfWeek, strTime);
                    int strcomtime = Convert.ToInt32(dtCurDate.TimeOfDay.Hours.ToString("0#") + "" + dtCurDate.TimeOfDay.Minutes.ToString("0#"));

                    if (isTimeSheet != null && isTimeSheet._end_time >= strcomtime && isTimeSheet._start_time <= strcomtime)
                    {
                        r.isRosterAvailable = true;
                        r._start_time = isTimeSheet._start_time;
                        r._end_time = isTimeSheet._end_time;
                    }
                    else
                    {
                        r.isRosterAvailable = false;
                    }
                });

                if (FEList.Count > 0 && !FEList.Any(a => a.isRosterAvailable == true))
                {
                    response.status = StatusCodes.OK.ToString();
                    response.message = FEList.Count + " FEs found but no roster available, ticket assign to: " + managerName;
                    logMessage = "Ticket-" + obRequest.tt_id + ": " + FEList.Count + " FEs found but no roster available, ticket assign to: " + managerName;
                    logHelper.ApiLogWriter("createTTJobOrder()", "TicketController", logMessage, null);
                    return response;
                }

                var feList = FEList.Where(f => f.isRosterAvailable == true).ToList();
                List<FEList> AvailableEF = new List<FEList>();
                foreach (var item in feList)
                {
                    var frtCountDetail = BLWFMTicket.GetTaskTrackingDetail(item.UserId);
                    item.fecount = frtCountDetail.Count;
                    AvailableEF.Add(item);
                }

                AvailableEF = AvailableEF.OrderBy(j => j.fecount).ToList();
                #endregion
                if (AvailableEF.Count > 0)
                {
                    int assignedFEid = 0;
                    for (int i = 0; i < AvailableEF.Count; i++)
                    {
                        int FEid = Convert.ToInt32(AvailableEF[i].UserId);


                        BLUser objBLuser = new BLUser();
                        Models.User objUserDetails = objBLuser.getUserDetails(FEid);
                        int capacity = Convert.ToInt32(objUserDetails.capacity == null ? 0 : objUserDetails.capacity);
                        List<FRTCapacity> FRTCapacityDetail = new List<FRTCapacity>();
                        FRTCapacityDetail = BLWFMTicket.GetTaskTrackingDetail(FEid);

                        if (FRTCapacityDetail.Count < capacity)
                        {
                            assignedFEid = FEid;
                            objMRIA.issueId = objRouteIssue.issue_id;
                            objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                            objMRIA.user_id = managerId;
                            objMRIA.frtUserId = assignedFEid;// Convert.ToInt32(FEList.FirstOrDefault().UserId);
                            objMRIA.status = "Assigned";
                            objMRIA.remarks = "Automatically Assigned Ticket";
                            objMRIA.assignedDate = DateTime.Now;
                            logMessage = logMessage + " Ticket assign to " + objUserDetails.user_name;
                            break;
                        }
                        logMessage = "  " + logMessage + "Capacity not available for " + objUserDetails.user_name + ", ";

                    }
                    if (assignedFEid == 0)
                    {
                        objMRIA.issueId = objRouteIssue.issue_id;
                        objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                        objMRIA.user_id = managerId;
                        objMRIA.frtUserId = managerId;
                        objMRIA.status = "UnAssigned";
                        objMRIA.remarks = "Assigned Ticket To Manager";
                        objMRIA.assignedDate = DateTime.Now;
                        message = "Job order created successfully and FRT capacity not available, ticket assign to: ";
                        //  logMessage = logMessage + " Capacity not available "+ managerName;
                    }

                }
                else
                {
                    objMRIA.issueId = objRouteIssue.issue_id;
                    objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                    objMRIA.user_id = managerId;
                    objMRIA.frtUserId = managerId;
                    objMRIA.status = "UnAssigned";
                    objMRIA.remarks = "Assigned Ticket To Manager";
                    objMRIA.assignedDate = DateTime.Now;
                }
            }
            else
            {
                objMRIA.issueId = objRouteIssue.issue_id;
                objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                objMRIA.user_id = managerId;
                objMRIA.frtUserId = managerId;
                objMRIA.status = "UnAssigned";
                objMRIA.remarks = "Assigned Ticket To Manager";
                objMRIA.assignedDate = DateTime.Now;
            }
            if (!SaveRouteIssueStatus(objMRIA))
            {
                response.message = "Unable to Assign ticket to frt. Manager needs to assign ticket " + obRequest.tt_id + " to FRT manually";
                response.status = strStatus;
                Exception ex = new Exception(response.status + ":" + response.message);
                return response;

            }
            User_Master userName = BLWFMTicket.GetUserById(objMRIA.frtUserId);
            response.message = message + userName.user_name;
            response.status = StatusCodes.OK.ToString();
            logMessage = "Ticket-" + obRequest.tt_id + ": " + logMessage + ", Ticket assign to " + userName.user_name;
            logHelper.ApiLogWriter("createTTJobOrder()", "TicketController", logMessage, null);


            return response;
        }




        private bool CreateHPSMTTTicket(CreateTTJob objobRequest, ref string strMessage, ref string strStatus, ref int ticket_tran_id, string jobOrderId = "", string parentJobOrderId = "", string serviceType = "", string service = "")
        {
            bool blnValue = false;

            try
            {
                Models.WFM.Task objTask = new Models.WFM.Task();
                objTask.hpsmid = jobOrderId;
                if (parentJobOrderId != "")
                {
                    objTask.parent_hpsmid = parentJobOrderId;
                }

                objTask.hpsmstatus = "Assigned";
                objTask.status = "Assigned";
                objTask.isresolve = "0";

                if (!string.IsNullOrEmpty(objobRequest.latitude) && !string.IsNullOrEmpty(objobRequest.longitude))
                //if (objobRequest.latitude != "" && objobRequest.longitude != "")
                {
                    objTask.latitude = Convert.ToString(objobRequest.latitude);
                    objTask.longitude = Convert.ToString(objobRequest.longitude);
                }


                objTask.comment_ = "Assignment";
                objTask.tasktype = objobRequest.tt_type;
                objTask.taskcategory = objobRequest.tt_category;
                objTask.tasksubcategory = objobRequest.tt_subtype;
                objTask.created_on = DateTime.Now;
                objTask.ticket_type = "";
                objTask.ticket_source_id = 4;
                objTask.addressid = objobRequest.address.addressid;
                objTask.addressline1 = objobRequest.address.address_line1;
                objTask.addressline2 = objobRequest.address.address_line2;
                objTask.addressline3 = objobRequest.address.address_line3;
                objTask.addresstype = objobRequest.address.address_type;
                objTask.city = objobRequest.address.city;
                objTask.pincode = objobRequest.address.pincode;
                objTask.stateorprovince = objobRequest.address.stateorprovince;
                objTask.addressline4 = objobRequest.address.address_line4;
                objTask.addressline5 = objobRequest.address.address_line5;
                objTask.package_name = objobRequest.package;
                objTask.nap_port = objobRequest.nap_port;
                objTask.node = objobRequest.node;
                objTask.cpe_brand = objobRequest.cpe_brand;
                objTask.cpe_model = objobRequest.cpe_model;
                objTask.current_cpesn = objobRequest.cpe_serialno;
                objTask.cpe_serialno = objobRequest.cpe_serialno;
                objTask.customer_category = objobRequest.customer_category;
                objTask.customer_segment = objobRequest.customer_segment;
                objTask.bandwidth = objobRequest.bandwidth;
                objTask.msp_cic_identifier = objobRequest.msp_cic_Identifier;
                objTask.secondary_contact = objobRequest.secondary_contact;
                objTask.email_id = objobRequest.email;
                objTask.customer_rmn = objobRequest.primary_contact;
                objTask.customer_id = objobRequest.customer_id;
                objTask.subscriber_name = objobRequest.customer_name;
                objTask.account_number = objobRequest.account_number;
                objTask.issue_comment = objobRequest.remarks;
                objTask.site_type = objobRequest.circuit_id;
                objTask.facility = objobRequest.service_type;
                objTask.cpe_mac_address = objobRequest.cpe_macaddress;
                objTask.action = "add";
                objTask.product_instance_id = objobRequest.product_instance_id;
                objTask.cpe_item_code = objobRequest.cpe_item_code;
                objTask.old_cpe_item_code = objobRequest.cpe_item_code;
                objTask.cpe_ref_serial = objobRequest.cpe_macaddress;
                //    objTask.

                if (!BLWFMTicket.InsertHpsm_TicketMasterData(objTask))
                {
                    strMessage = "Unable save to new job order!!";
                    strStatus = StatusCodes.INVALID_REQUEST.ToString();
                    return blnValue;
                }
                ticket_tran_id = objTask.hpsm_ticket_id;
                blnValue = true;
            }
            catch (Exception ex)
            {
                strMessage = "Error While Processing  Request.";
                strStatus = StatusCodes.INVALID_REQUEST.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(objobRequest);
                logHelper.ApiLogWriter("CreateHPSMTicket()", "TicketController", request, ex);
            }
            return blnValue;
        }
        private bool CreateTTRouteIssue(CreateTTJob obRequest, ref Route_Issue route_issue, ref string strMessage, ref string strStatus, int ticket_tran_id, ref int route_issue_id, int managerId)
        {
            bool blnValue = false;
            try
            {

                Route_Issue objRI = new Route_Issue();
                objRI.issue_id = 0;
                //objRI.issue_type_id = issue_type.oid;

                objRI.issue_type_id = 0;

                objRI.latitude = Convert.ToDouble(obRequest.latitude); //objRouteIssueVM.routeIssueInput.latitude;
                objRI.longitude = Convert.ToDouble(obRequest.longitude);
                objRI.hpsm_ticketid = ticket_tran_id;

                //need to look this after fine tune 
                objRI.route_assignment_id = 0;
                objRI.user_id = managerId;
                objRI.manager_id = managerId;
                objRI.status = "UnAssigned";
                objRI.user_remark = objRI.issue_remark;
                objRI.modified_on = DateTime.Now;
                objRI.manager_remark = "";
                objRI.mobile_time = DateTime.Now;
                objRI.circle_id = 0;

                if (!BLWFMTicket.SaveRouteIssue(objRI))
                {
                    strMessage = "Unable to save route issues!";
                    strStatus = StatusCodes.INVALID_REQUEST.ToString();
                    return blnValue;
                }
                route_issue = objRI;
                route_issue_id = route_issue.issue_id;
                blnValue = true;
            }
            catch (Exception ex)
            {
                strMessage = "Error While Processing  Request.";
                strStatus = StatusCodes.INVALID_REQUEST.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                logHelper.ApiLogWriter("CreateTTRouteIssue()", "TicketController", request, ex);
            }
            return blnValue;
        }

        [HttpPost]
        [Route("GetStatusDetailByJobOrderId")]
        [ValidateModel]
        public WFMApiResponse<dynamic> GetStatusDetailByJobOrderId(getDetailIn obRequest)
        {
            var response = new WFMApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    getStatusDetail getStatusDetail = BLWFMTicket.GetStatusDetailByJobOrderId(obRequest);
                    if (getStatusDetail != null)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.results = getStatusDetail;

                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.message = "No record found.";
                    }

                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("GetStatusDetailByJobOrderId()", "TicketController", obRequest.job_id, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.message = "Error While Processing  Request.";
                }
            }

            return response;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("CreateCustomerTicket")]
        [Filters.CustomActionForXml]
        public IHttpActionResult CreateCustomerTicket(CustomerTicketMaster objTicketMaster)
        {
           
            try
            {
                if (ModelState.IsValid)
                {
                    var result = new BLTicketManager().SaveCustomerTicket(objTicketMaster);
                    if (result != null)
                    {
                        var responses = new Customer_Response
                        {
                            reference_id = result.reference_id, // Assuming you generate a new reference_id
                            ticketid = result.ticketid
                        };

                        return Json(responses);
                    }
                    else
                    {
                        var errorResponse = new ErrorResponse
                        {
                            code = (int)HttpStatusCode.InternalServerError,
                            message = "Data not saved successfully"
                        };
                        return Content(HttpStatusCode.BadRequest, errorResponse);
                    }
                }
                else
                {
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    var errorMessageString = string.Join("", errorMessages);
                    var errorResponse = new ErrorResponse
                    {
                        code = (int)HttpStatusCode.BadRequest,
                        message = errorMessageString
                    };
                    return Content(HttpStatusCode.BadRequest, errorResponse);
                }
            }
            catch (Exception ex)
            {
                // Log exception details
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("CreateCustomerTicket", "TicketManager", objTicketMaster?.ToString(), ex);

                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.InternalServerError,
                    message = "Error while processing request."
                };
                return Content(HttpStatusCode.BadRequest, errorResponse);

            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("customerTicketStatus")]
        [Filters.CustomActionForXml]
        public IHttpActionResult customerTicketStatus(string ticket_id)
        {
            var response = new ApiResponse<customerTicketStatus>();
            customerTicketStatus customerTicketStatus = new customerTicketStatus();
            try
            {
                if ((!string.IsNullOrEmpty(ticket_id)) && !ContainsSpecialCharacters(ticket_id) && !IsNumberOnly(ticket_id))
                {

                        customerTicketStatus = new BLTicketManager().GetcustomerTicketStatus(Convert.ToInt32(ticket_id));
                        if (customerTicketStatus != null && !string.IsNullOrEmpty(customerTicketStatus.ticket_id))
                        {
                            var responseData = new
                            {
                                ticket_id = customerTicketStatus.ticket_id,
                                status = customerTicketStatus.ticket_status,
                                can_id = customerTicketStatus.can_id,
                                created_on = customerTicketStatus.created_on,
                                assigned_to = customerTicketStatus.assigned_to,
                                assigned_on = customerTicketStatus.assigned_date,
                                completed_on = customerTicketStatus.target_date,
                                remarks = customerTicketStatus.remarks
                            };
                            return Json(responseData);
                        }
                        else
                        {
                            var errorResponse = new ErrorResponse
                            {
                                code = (int)HttpStatusCode.NotFound,
                                message = "Data not found"
                            };
                            return Content(HttpStatusCode.NotFound, errorResponse);
                        }

                    
                }
                else
                {
                    var errorResponse = new ErrorResponse
                    {
                        code = (int)HttpStatusCode.BadRequest,
                        message = "Ticket Id are not valid"
                    };
                    return Content(HttpStatusCode.BadRequest, errorResponse);

                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("customerTicketStatus", "TicketManager", customerTicketStatus?.ToString(), ex);
                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.InternalServerError,
                    message = "Error while processing request."
                };
                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
        }
       
        public static bool ContainsSpecialCharacters(string input)
        {
            // Define a regex pattern that allows only alphanumeric characters
            // You can add more allowed characters to the pattern if needed
            string pattern = "^[a-zA-Z0-9]*$";
            return !Regex.IsMatch(input, pattern);
        }
        public static bool IsNumberOnly(string input)
        {
            // Define a regex pattern that allows only digits
            string pattern = "^[0-9]+$";

            // Check if the input matches the pattern
            return !Regex.IsMatch(input, pattern);
        }

    }
}