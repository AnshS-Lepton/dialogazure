using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility
{
    public class BrowserHelper
    {
        string _BrowserName { get; set; }
        string _BrowserType { get; set; }
        string _BrowserVersion { get; set; }

        public BrowserHelper()
        {
            GetBrowserInfo();
        }

        public void GetBrowserInfo()
        {
            HttpBrowserCapabilities browser = null;
            try
            {
                browser = HttpContext.Current.Request.Browser;
                var Browser = HttpContext.Current.Request.UserAgent;

                if (Browser.Contains("Edg"))
                {
                    _BrowserName = "Edge";
                }
                else if (Browser.Contains("Opr"))
                {
                    _BrowserName = "Opera";
                }
                else if (Browser.Contains("Firefox"))
                {
                    _BrowserName = "Firefox";
                }
                else if (Browser.Contains("Chrome"))
                {
                    _BrowserName = "Chrome";
                }
                else
                {
                    _BrowserName = "Other";
                }
                _BrowserType = browser.Type;
                _BrowserVersion = browser.Version;
            }
            catch
            {
                _BrowserType = "";
                _BrowserType = "Other";
            }
            //_BrowserName = browser.Browser;
        }

        public string BrowserName { get { return _BrowserName; } }
        public string BrowserType { get { return _BrowserType; } }
        public string BrowserVersion { get { return _BrowserVersion; } }

    }
}
