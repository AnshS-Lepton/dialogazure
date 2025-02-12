using BusinessLogics;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;
using Models.API;


namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class RingDetailsController : Controller
    {
        // GET: RingDetails
        public ActionResult ShowTopologyRingDetails(RingDetailsFiltter objRingFilter, int page = 0, string sort = "", string sortdir = "")
        {
            string SearchVar = "";
            string network_id = "";
            objRingFilter.objGridAttributes.pageSize = 10;
            objRingFilter.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objRingFilter.objGridAttributes.sort = sort;
            objRingFilter.objGridAttributes.orderBy = sortdir;
            if ((objRingFilter.objGridAttributes.searchBy == "network_id" || objRingFilter.objGridAttributes.searchBy == "region_name")
                && !string.IsNullOrEmpty(objRingFilter.objGridAttributes.searchText))
            {
                SearchVar = objRingFilter.objGridAttributes.searchText;
                objRingFilter.objGridAttributes.searchText = "";
                objRingFilter.objGridAttributes.currentPage = 0;
            }
            if (objRingFilter.objRingDetails != null)
            {
                if (!string.IsNullOrEmpty(objRingFilter.objRingDetails.region_name) && !string.IsNullOrEmpty(objRingFilter.objRingDetails.segment_name) && !string.IsNullOrEmpty(objRingFilter.objRingDetails.ring_name))
                {
                    network_id = objRingFilter.objRingDetails.region_name + "-" + objRingFilter.objRingDetails.segment_name + "-" + objRingFilter.objRingDetails.ring_name;
                }
                else
                {
                    if (!string.IsNullOrEmpty(objRingFilter.objRingDetails.region_name))
                    {
                        network_id = "%"+objRingFilter.objRingDetails.region_name+"%";

                    }
                    if (!string.IsNullOrEmpty(objRingFilter.objRingDetails.segment_name))
                    {
                        if (!string.IsNullOrEmpty(network_id))
                        {
                            network_id = network_id + "-%" + objRingFilter.objRingDetails.segment_name+"%";
                        }
                        else
                        {
                            network_id = "%"+objRingFilter.objRingDetails.segment_name+"%";
                        }
                    }
                    if (!string.IsNullOrEmpty(objRingFilter.objRingDetails.ring_name))
                    {
                        if (!string.IsNullOrEmpty(network_id))
                        {
                            network_id = network_id + "-%" + objRingFilter.objRingDetails.ring_name+"%";
                        }
                        else
                        {
                            network_id = "%"+objRingFilter.objRingDetails.ring_name+"%";
                        }
                    }
                }

            }
            var ringdetails = new BLRingDetails().getRingDetails(objRingFilter.objGridAttributes, network_id);

            objRingFilter.lstRingDetails = ringdetails;
            objRingFilter.objGridAttributes.totalRecord = objRingFilter.lstRingDetails != null && objRingFilter.lstRingDetails.Count > 0 ? SearchVar != "" ? objRingFilter.lstRingDetails.Count : objRingFilter.lstRingDetails[0].totalRecords : 0;
            objRingFilter.lstRegionName = new BLRingDetails().GetRegionDetails();
            objRingFilter.lstSegmentName = new BLRingDetails().GetSegmentDetails();
            objRingFilter.lstRingType = new BLRingDetails().GetRingTypeDetails();

            return PartialView("_ShowTopologyRingDetails", objRingFilter);
        }
    }
}