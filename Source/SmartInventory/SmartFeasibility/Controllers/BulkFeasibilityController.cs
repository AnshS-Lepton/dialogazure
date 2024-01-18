using ExcelDataReader;
using Newtonsoft.Json;
using SmartFeasibility.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SmartFeasibility.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class BulkFeasibilityController : Controller
    {
        // GET: BulkFeasibility
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Ftth()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                DataTable dataTable = new DataTable();
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        Stream stream = file.InputStream;
                        IExcelDataReader reader = null;

                        reader = ExcelReaderFactory.CreateReader(stream);

                        var conf = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };

                        var dataSet = reader.AsDataSet(conf);

                        if (dataSet != null && dataSet.Tables.Count > 0)
                        {
                            dataTable = dataSet.Tables[0];
                            return Json(new { status = "success", result = JsonConvert.SerializeObject(dataTable) });
                        }
                        reader.Close();
                    }
                }
                return Json(new { status = "empty" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "fail", error = ex.Message });
            }
        }
    }
}