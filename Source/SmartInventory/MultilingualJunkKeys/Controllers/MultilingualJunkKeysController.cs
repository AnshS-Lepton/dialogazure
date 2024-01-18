using BusinessLogics;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MultilingualJunkKeys.Controllers
{
    public class MultilingualJunkKeysController : Controller
    {
        // GET: MultilingualJunkKeys
        public ActionResult Index()
        {
            return View();
        }

        public void CheckjunkKey()
        {
            var startFolder = ConfigurationManager.AppSettings["JunkKeysPath"];
            startFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"" + startFolder + ""));
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Where(file => new string[] { ".js", ".cs", ".cshtml" }.Contains(Path.GetExtension(file.ToString()))).ToList();
            var keylist = new BLResources().GetResourceKeyList();
            List<ResourcesKeyStatus> lstResources = new List<ResourcesKeyStatus>();
            foreach (FileInfo files in fileList)
            {
                foreach (var item in keylist)
                {
                    string fileContents = String.Empty;
                    if (System.IO.File.Exists(files.FullName) && files.Name != "Resources.cs")
                    {
                        fileContents = System.IO.File.ReadAllText(files.FullName);
                    }
                    var fileStatus = fileContents.Contains(item);
                    ResourcesKeyStatus objResKeyStatus = new ResourcesKeyStatus();
                    if (fileStatus)
                    {
                        objResKeyStatus.key = item;
                        objResKeyStatus.status = fileStatus;
                        if (files.Extension == ".cs")
                            objResKeyStatus.source_name = files.Directory.Parent.Name + "/" + files.Name;
                        else
                            objResKeyStatus.source_name = files.Directory.Parent.Parent.Name + "/" + files.Directory.Parent.Name + "/" + files.Name;
                        objResKeyStatus.created_by = 1;
                        objResKeyStatus.created_on = DateTimeHelper.Now;
                        lstResources.Add(objResKeyStatus);
                    }
                }
            }
            if (lstResources.Count > 0)
            {
                BLResources.Instance.Delete_Junk_Key();
                BLResources.Instance.CheckJunkKeys(lstResources);
                BLResources.Instance.checkFunctionJunk_Key();
            }
        }
    }
}