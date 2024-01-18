using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SmartInventory.Helper
{
    public class CustomHtmlHelper
    {
        public static IDictionary<string, object> GetHtmlAttributes(object fixedHtmlAttributes = null, object dynamicHtmlAttributes = null)
        {


            var rvd = (fixedHtmlAttributes == null)
                ? new RouteValueDictionary()
                : HtmlHelper.AnonymousObjectToHtmlAttributes(fixedHtmlAttributes);
            if (dynamicHtmlAttributes != null)
            {
                Type myType = dynamicHtmlAttributes.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                foreach (PropertyInfo prop in props)
                {
                    object propValue = prop.GetValue(dynamicHtmlAttributes, null);
                    rvd[prop.Name] = propValue;
                }
            }
            return rvd;
        }
    }
}