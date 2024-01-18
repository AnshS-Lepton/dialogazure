using Lepton.Entities;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utility;
using Utility.BlUtility;

namespace Lepton.Utility
{
    public static class WebAPIRequest
    {
        static readonly string baseAPIUrl = null;
        static WebAPIRequest()
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SmartInventoryServiceURL"]))
            {
                baseAPIUrl = new Uri(ConfigurationManager.AppSettings["SmartInventoryServiceURL"]).ToString();
            }

        }

        #region Generate Api Token 
        /// <summary>
        /// Get Api Token On Server Side
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>Token For Authentication</returns>
        /// <Created By>Sumit Poonia</Created By>
        public static TokenDetail GetAPIToken(string userName, string password)
        {
            try
            {
                var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "Password", password ),
                        new KeyValuePair<string, string> ( "source", "WEB" )
                    };
                var content = new FormUrlEncodedContent(pairs);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(baseAPIUrl + "token", content).Result;
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        //LogHelper.GetInstance.WriteDebugLog("response.StatusCode" + response.StatusCode);
                        return null;
                    }
                    else
                    {
                        //LogHelper.GetInstance.WriteDebugLog("else response.StatusCode" + response.StatusCode);
                        var result = response.Content.ReadAsStringAsync().Result;
                        //LogHelper.GetInstance.WriteDebugLog("Token Response:" + result.ToString());
                        return JsonConvert.DeserializeObject<TokenDetail>(result);
                    }
                }
            }
            catch
            {
                //LogHelper.GetInstance.WriteDebugLog("Exception Block");
                return null;
            }
        }

       public static TokenDetail GetRefreshToken(string refreshToken,string accessToken)
        {
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "refresh_token" ),
                        new KeyValuePair<string, string>( "refresh_token", refreshToken )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var response = client.PostAsync(baseAPIUrl + "token", content).Result;
                return JsonConvert.DeserializeObject<TokenDetail>(response.Content.ReadAsStringAsync().Result);
            }
        }
        #endregion

        #region Get Api Request
        /// <summary>
        /// Get Api Request by Token
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Request Detail</returns>
        ///<Created By>Sumit Poonia</Created By>
        public static string GetAPIRequest(string url)
        {
            var token = (TokenDetail)System.Web.HttpContext.Current.Session["TokenDetail"];
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token.access_token))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
                    if (System.Web.HttpContext.Current.Session["Language"] != null)
                        client.DefaultRequestHeaders.Add("Languge", System.Web.HttpContext.Current.Session["Language"].ToString());
                }
                var response = client.GetAsync(baseAPIUrl + url).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public static string GetAPIOTPRequest(string url)
        {
            var token = (TokenDetail)System.Web.HttpContext.Current.Session["OTPTokenDetail"];
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token.access_token))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
                    if (System.Web.HttpContext.Current.Session["Language"] != null)
                        client.DefaultRequestHeaders.Add("Languge", System.Web.HttpContext.Current.Session["Language"].ToString());
                }
                var response = client.GetAsync(baseAPIUrl + url).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        #endregion

        #region Post Api Request
        /// <summary>
        /// Get Api Request by Token
        /// </summary>
        /// <returns>Request Detail</returns>
        ///<Created By>Sumit Poonia</Created By>
        public static ApiResponse<V> PostIntegrationAPIRequest<V>(string url, dynamic obj, string Entity_Type = null, string Entity_Action = null, string structur_id = null, TokenDetail _token = null, bool islanguagereq = true) where V : class, new()
        {
            TokenDetail token = new TokenDetail();
            if (_token != null)
                token = !string.IsNullOrWhiteSpace(_token.access_token) ? _token : (TokenDetail)System.Web.HttpContext.Current.Session["TokenDetail"];
            else
                token = (TokenDetail)System.Web.HttpContext.Current.Session["TokenDetail"];
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token.access_token))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
                    //Set Multilingual Language by key value access in cust athorization by  key name
                    if(islanguagereq)
                        if (System.Web.HttpContext.Current.Session["Language"] != null)
                            client.DefaultRequestHeaders.Add("languge", System.Web.HttpContext.Current.Session["Language"].ToString());
                    client.DefaultRequestHeaders.Add("source", "WEB");
                    client.DefaultRequestHeaders.Add("source_ref_id", "");
                    client.DefaultRequestHeaders.Add("source_ref_type", "");
                    client.DefaultRequestHeaders.Add("source_ref_description", "");
                    client.DefaultRequestHeaders.Add("entity_Type", Entity_Type);
                    client.DefaultRequestHeaders.Add("entity_Action", Entity_Action);
                    client.DefaultRequestHeaders.Add("structure_id", structur_id);
                }
                var json = new { data = JsonConvert.SerializeObject(obj) };
                string jsonData = JsonConvert.SerializeObject(json);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = client.PostAsync(baseAPIUrl + url, content).Result;
                return JsonConvert.DeserializeObject<ApiResponse<V>>(response.Content.ReadAsStringAsync().Result);
            }
        }
        #endregion

        #region Get Token Request
        /// <summary>
        /// Get Api Request by Token
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Request Detail</returns>
        ///<Created By>Sumit Poonia</Created By>
        public static string GetAPIRequestWithClientToken(string url, string token)
        {
            try
            {
                using (var client = GetClient(token))
                {
                    var response = client.GetAsync(baseAPIUrl + url).Result;
                    //Lepton.Utility.ErrorLog.LogErrorToLogFile(null, response + "GetAPIRequestWithClientToken, line 75");
                    var x = response.Content.ReadAsStringAsync().Result;
                    //Lepton.Utility.ErrorLog.LogErrorToLogFile(null, x + "GetAPIRequestWithClientToken, line 77");
                    return x;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        /// <summary>
        /// get request for client side token from server side
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetIntegrationAPIRequestWithClientToken(string url, string token)
        {
            try
            {
                using (var client = GetClient(token))
                {
                    var response = client.GetAsync(baseAPIUrl + url).Result;
                    var x = response.Content.ReadAsStringAsync().Result;
                    return x;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        #region Auth with bearer token
        /// <summary>
        /// Get Api Request by Token
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Request Detail</returns>
        ///<Created By>Sumit Poonia</Created By>
        public static HttpClient GetClient(string token)
        {
            var authValue = new AuthenticationHeaderValue("Bearer", token);
            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
                //Set some other client defaults like timeout / BaseAddress
            };
            return client;
        }
        #endregion


        #region Post Api Request
        ///<Created By>Antra Mathur</Created By>
        public static T PostPartnerAPI<T>(int userId, string gisdesignId, int systemId, string entityType, dynamic obj, string URL, string InputParameter="", int objectId=-1, string InputType="", string WhereCondition="", string TransactionId="",string processId="")
        {
            PartnerAPILog log = new PartnerAPILog();
            PUSHLOGS _obj = new PUSHLOGS();
            GisApiLogs objGisApiLogs = new GisApiLogs();

            var json = "";
            var formdata = new MultipartFormDataContent();
            var GISMessage = "Please contact to GIS administrator. ";

            try
            {
                var indate = DateTime.Now;          
                var client = new HttpClient();
                HttpContent content = null;
                HttpResponseMessage response = new HttpResponseMessage();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
               
                //To save data in GisApiLogs
                objGisApiLogs.api_url = URL;
                objGisApiLogs.request = json;
                objGisApiLogs.request_time = DateTime.Now;
                if (InputType.ToUpper() == "FETCHGISDATA")
                {
                    //formdata.Add(new StringContent(objectId.ToString()), "objectids");
                    if (!string.IsNullOrEmpty(InputParameter))
                    {
                        formdata.Add(new StringContent(InputParameter), "gdbVersion");
                    }
                    formdata.Add(new StringContent("json"), "f");
                    formdata.Add(new StringContent("*"), "outFields");
                    formdata.Add(new StringContent(WhereCondition), "Where");
                    response = client.PostAsync(URL, formdata).Result;         
                    objGisApiLogs.api_type = "GIS";
                    _obj.WHERE = WhereCondition;
                    if (!string.IsNullOrEmpty(InputParameter))
                    {
                        _obj.gdbVersion = InputParameter.ToString();
                    }
                    json = JsonConvert.SerializeObject(_obj);
                }
                else if (InputType.ToUpper() == "FORM-DATA")
                {                  
                    json = JsonConvert.SerializeObject(obj);
                    formdata.Add(new StringContent("json"), "f");
                    formdata.Add(new StringContent(json), WhereCondition);
                    formdata.Add(new StringContent(InputParameter), "gdbVersion");
                    response = client.PostAsync(URL, formdata).Result;
                    objGisApiLogs.api_type = "GIS";
                    _obj.applyEdits = WhereCondition;
                    _obj.gdbVersion=InputParameter.ToString();
                    _obj.fields = obj;
                    json = JsonConvert.SerializeObject(_obj);
                }
                else if (InputType.ToUpper() == "DELETE-DATA")
                {
                    formdata.Add(new StringContent(objectId.ToString()), "objectids");
                    formdata.Add(new StringContent("json"), "f");
                    formdata.Add(new StringContent(InputParameter), "gdbVersion");
                    response = client.PostAsync(URL, formdata).Result;
                    objGisApiLogs.api_type = "GIS";
                    _obj.gdbVersion = InputParameter;
                    _obj.objectId = objectId.ToString();
                    json = JsonConvert.SerializeObject(_obj);
                }
                else if (InputType.ToUpper() == "NAS-FORM-DATA")
                {
                    json = JsonConvert.SerializeObject(obj);
                    var fileContent = new ByteArrayContent(obj.FormFile, 0, obj.FormFile.Length);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

                    formdata.Add(fileContent, "FormFile", obj.xm_file_name);
                    formdata.Add(new StringContent(obj.CSAID), "CSAID");
                    formdata.Add(new StringContent(obj.inserted_by), "INSERTED_BY");
                    formdata.Add(new StringContent(obj.system), "SYSTEM");
                    formdata.Add(new StringContent(obj.version_name), "version_name");
                    response = client.PostAsync(URL, formdata).Result;
                    objGisApiLogs.api_type = "NAS";

                }
                else
                {
                    json = JsonConvert.SerializeObject(obj);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = client.PostAsync(URL, content).Result;
                    objGisApiLogs.api_type = "GIS";
                }

                objGisApiLogs.response_time = DateTime.Now;


                #region To Maintain Logs
                var LogResp = JsonConvert.DeserializeObject<PartnerAPILog>(response.Content.ReadAsStringAsync().Result);
                var outdate = DateTime.Now;
                log.URL = URL;
                log.request = json;
                LogResp.InDateTime= indate.ToString();
                LogResp.OutDateTime= outdate.ToString();
                log.response = response.Content.ReadAsStringAsync().Result;
                log.Transactionid = TransactionId;
                log.InDateTime = LogResp.InDateTime;
                log.OutDateTime = LogResp.OutDateTime;
                log.Latency = LogResp.Latency;
                //log.UserName = usrDetail.user_name;

                //To save data of log in GisApiLogs
                objGisApiLogs.response = response.Content.ReadAsStringAsync().Result;
               // objGisApiLogs.response_time = DateTime.Now;  //response will get the input at the time of response so shifted above
                objGisApiLogs.latency = Convert.ToInt32(objGisApiLogs.response_time.Value.Second - objGisApiLogs.request_time.Value.Second);
                objGisApiLogs.transaction_id = TransactionId;
                objGisApiLogs.user_id =Convert.ToInt32(userId);
                objGisApiLogs.gis_object_id = objectId;
                objGisApiLogs.gdb_version = InputParameter;
                objGisApiLogs.gis_design_id = gisdesignId;
                objGisApiLogs.entity_type = entityType;
                objGisApiLogs.system_id = systemId;
                objGisApiLogs.process_id = processId;
                var _GISCreateVersionLogResp = JsonConvert.DeserializeObject<CreateVersionOut>(response.Content.ReadAsStringAsync().Result);
                if (_GISCreateVersionLogResp.Status != null)
                {
                    if (_GISCreateVersionLogResp.Status.ToUpper() == "SUCCESS" || _GISCreateVersionLogResp.Status.ToUpper() == "VERSION ALREADY EXISTS" || _GISCreateVersionLogResp.Status.Contains("same Name is already exist in the database"))
                    {
                        objGisApiLogs.message = _GISCreateVersionLogResp.Status;
                        objGisApiLogs.status = "Success";
                    }
                    else
                    {
                        objGisApiLogs.message = _GISCreateVersionLogResp.Status;
                        objGisApiLogs.status = "Failed";
                    }
                }

                var _LogResp = JsonConvert.DeserializeObject<GISAPIResponse>(response.Content.ReadAsStringAsync().Result);
                if(_LogResp.addResults!=null) {
                    if (_LogResp.addResults.Count > 0) {
                        objGisApiLogs.status = "Success";
                        objGisApiLogs.gis_object_id = _LogResp.addResults[0].objectId; 
                        objGisApiLogs.message = "Pushed Successfully.";
                    }
                }
                if (_LogResp.updateResults!=null) { 
                    if (_LogResp.updateResults.Count > 0) {
                        objGisApiLogs.status = "Success";
                        objGisApiLogs.gis_object_id = _LogResp.updateResults[0].objectId;
                        objGisApiLogs.message = "Pushed Successfully.";
                    } 
                }
                if (_LogResp.deleteResults!=null) { 
                    if (_LogResp.deleteResults.Count > 0) { 
                        objGisApiLogs.status = "Success";
                    }
                }
                if (_LogResp.error != null) { 
                    if (_LogResp.error.details != null) { 
                        if (_LogResp.error.details.Count > 0) { 
                            objGisApiLogs.status = "Failed"; 
                            objGisApiLogs.message = GISMessage + _LogResp.error.message;
                        } 
                    }
                }


                var _GISPostVersionLogResp = JsonConvert.DeserializeObject<PostVersionOut>(response.Content.ReadAsStringAsync().Result);
                if (_GISPostVersionLogResp.Status != null)
                {
                   // objGisApiLogs.message = _GISPostVersionLogResp.Message;
                    objGisApiLogs.status = _GISPostVersionLogResp.Status.ToCamelCase();
                }
                new BLErrorLog().SaveGisApiLogs(objGisApiLogs);

                ErrorLogHelper logHelper = new ErrorLogHelper();
                //logHelper.PartnerAPILogs(log);
                #endregion
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
            catch(Exception ex)
            {
                log.URL = URL;
                log.request = json;
                log.response = ex.Message;
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.PartnerAPILogs(log);
                var x= "{\"message\":"+ex.Message+"}";
                return JsonConvert.DeserializeObject<T>(x);
            }
        }

    }
    #endregion

}
