using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
    public class FiberLinkController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetFiberLinks(int user_id, int page = 0, string sort = "", string searchText = "")
        {
            FiberLinkFilter objFiberLinkFilter = new FiberLinkFilter();
            objFiberLinkFilter.pageSize = 10;
            objFiberLinkFilter.userid = user_id;
            objFiberLinkFilter.currentPage = page == 0 ? 1 : page;
            objFiberLinkFilter.orderBy = sort;
            objFiberLinkFilter.Searchtext = searchText;

            List<Dictionary<string, string>> lstFiberLinks = new BLFiberLink().GetFiberLinks(user_id, objFiberLinkFilter);

            string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
            foreach (Dictionary<string, string> dic in lstFiberLinks)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objFiberLinkFilter.lstFiberLinkDetails.Add(obj);
            }
            objFiberLinkFilter.lstFiberLinkDetails = BLConvertMLanguage.MultilingualConvert(objFiberLinkFilter.lstFiberLinkDetails, arrIgnoreColumns);

            objFiberLinkFilter.totalRecord = lstFiberLinks.Count > 0 ? Convert.ToInt32(lstFiberLinks[0].FirstOrDefault().Value) : 0;

            return Json(new
            {
                LstFiberLinkDetails = objFiberLinkFilter.lstFiberLinkDetails,
                TotalRecord = objFiberLinkFilter.totalRecord,
                PageSize = objFiberLinkFilter.pageSize,
                CurrentPage = objFiberLinkFilter.currentPage
            });
        }
        [HttpGet]
        public IHttpActionResult GetFiberLinksByLinkIds(string linkIds)
        {
            try
            {
                List<string> FiberLinkDetails = new BLFiberLink().GetFiberLinksByLinkIds(linkIds);
                return Json(new
                {
                    result = FiberLinkDetails.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetFiberLinksByLinkIds()", "FiberLink", ex);
                return Json(new { status = ResponseStatus.ERROR.ToString(), result = "" });
            }
        }
    }
}
