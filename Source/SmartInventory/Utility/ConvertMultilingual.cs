using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Globalization;

using Models;
using System;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace Utility
{
    public class ConvertMultilingual
    {

        private ConvertMultilingual()
        {

        }
       
        public static string GetLayerActionMessage(string Message, List<Models.layerDetail> listLayerDetails, params string[] Layer_Name)
        {
            List<string> list = new List<string>();
            foreach (var item in Layer_Name)
            {
                var title = listLayerDetails.Where(x => x.layer_name.Replace(" ", "").ToUpper() == item.ToUpper()).FirstOrDefault().layer_title;
                list.Add(title);
            }
            string result = String.Format(Message, list.ToArray());
            return result;
        }
}
}

