using BusinessLogics;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Models;
using Newtonsoft.Json;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;
using Utility.MapPrinter;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class MapLayoutController : Controller
    {
        [OutputCache(CacheProfile = "CacheForOneDay")]
        public ActionResult Index(MapLayout layout)
        {
           
            return PartialView("_ViewMapLayout", layout);
        }
    }
}