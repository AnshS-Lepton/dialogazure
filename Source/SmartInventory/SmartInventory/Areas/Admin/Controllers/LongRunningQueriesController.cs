using BusinessLogics;
using Models;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System.Web.Mvc;



namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class LongRunningQueriesController : Controller
    {
        // GET: Admin/LongRunningQueries
       


        public ActionResult ViewLongRunningQueries(ViewLongRunningQueries objViewLongRunningQueries, int page = 0, string sort = "", string sortdir = "")
        {
            if (sort != "" || page != 0)
            {
                objViewLongRunningQueries = (ViewLongRunningQueries)Session["longRunningQueries"];
            }
            objViewLongRunningQueries.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;//ViewLongRunningQueriesGridPageSize
            objViewLongRunningQueries.currentPage = page == 0 ? 1 : page;
            objViewLongRunningQueries.sort = sort;
            objViewLongRunningQueries.orderBy = sortdir;
            if (objViewLongRunningQueries.runningTime == 0)
            {
                objViewLongRunningQueries.runningTime = 2;
            }
            if (objViewLongRunningQueries.searchBy == null)
            {
                objViewLongRunningQueries.searchBy = "";
            }
            Session["viewLongRunningQueries"] = objViewLongRunningQueries;
            objViewLongRunningQueries.listLongRunningQueries = new LongRunningQuery().GetLongRunningQueriesData(objViewLongRunningQueries);
            objViewLongRunningQueries.totalRecord = objViewLongRunningQueries.listLongRunningQueries != null && objViewLongRunningQueries.listLongRunningQueries.Count > 0 ? objViewLongRunningQueries.listLongRunningQueries[0].totalRecords : 0;
            Session["longRunningQueries"] = objViewLongRunningQueries;
            return View(objViewLongRunningQueries);
        }

        public ActionResult ReadMoreLongRunningQueries(int id)
        {
            ViewLongRunningQueries objLongRunningQueries = new ViewLongRunningQueries();
            objLongRunningQueries.objLongRunningQueriesMore = new LongRunningQuery().getLongRunningQueryDetailById(id);
            objLongRunningQueries.logtype = "LongRunningQueries";
            return PartialView("_ReadMoreLongRunningQueries", objLongRunningQueries);
        }


        [HttpPost]
        public JsonResult TerminatequeryByPid(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                LongRunningQueries objLongRunningQueries = new LongRunningQueries();
                objLongRunningQueries.status = new  LongRunningQuery().TerminateQueryByPid(id);

                    if (objLongRunningQueries.status == true)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = "Query Terminated successfully!";
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = "Something went wrong while Terminating Query!";
                    }
                
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Query Not terminated!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

    }
}