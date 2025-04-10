
using BusinessLogics;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Models;
using Newtonsoft.Json;
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

namespace SmartInventory.Controllers
{

    [AllowAnonymous]
    public class SpxI49FtzController : Controller
    {
        // GET: FiberLink 

        public ActionResult Index()
        
        {

            var Data = new BLOSPSplicing().getSplicingRecord();
            return View(Data);
        }

    }
}