using Lepton.Utility;
using Models;
using Models.VectorLayers;
using Newtonsoft.Json;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Utility;
using static Mono.Security.X509.X520;

namespace SmartInventory.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class VectorLayerController : Controller
    {
        // GET: VectorLayer
        public ActionResult Index()
        {
            return View();
        }
        [Compression]
       
        public string GetVectorGeojson(string vectorPrvinceIds, string entityType)
        {
            DateTime requestTime = DateTimeHelper.Now;
            string _logFolderPaths = ConfigurationManager.AppSettings["logFolderPath"].ToString();
            string url = "api/VectorLayer/GetVectorData";
            var response = JsonConvert.SerializeObject(WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, new VectorDataIn { PrvinceIds = vectorPrvinceIds, entityType = entityType }));
           /* using (StreamWriter sw = System.IO.File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPaths + "VectorAPILog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
            {
                sw.WriteLine("\r\n VectorLayer:==========>");
                sw.WriteLine("WEB Request Time:{0}, Response Time:{1}", requestTime, DateTimeHelper.Now);
            }*/
            return response;
           /* var response = WebAPIRequest.GetAPIRequest(url);
            return response;*/
        }

        [Compression]
        public dynamic GetVectorDelta(string lastFetchTime, string vectorPrvinceIds, int vectorFSAID = 0)
        {
            string url = "api/VectorLayer/GetVectorDelta";
            //var response = WebAPIRequest.GetAPIRequest(url);
            var response = JsonConvert.SerializeObject(WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, new VectorDeltaIn { LastFetchTime = lastFetchTime, FSAId = vectorFSAID,  PrvinceIds = vectorPrvinceIds }));
            return response;
        }

        [Compression]
        public dynamic GetVectorProvinceData()
        {
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            string url = "api/VectorLayer/GetVectorProvinceData";          
            var response = JsonConvert.SerializeObject(WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, new VectorProvinceDataIn { UserId = usrId }));
            return response;
        }
        [Compression]
        public dynamic GetVectorEntityStyle()
        {
            string url = "api/VectorLayer/GetVectorEntityStyle";
            var response = JsonConvert.SerializeObject(WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, new { }));
            return response;
        }
    }
}