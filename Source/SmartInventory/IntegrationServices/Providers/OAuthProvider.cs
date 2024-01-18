using BusinessLogics;
using DataAccess;
using IntegrationServices.Settings;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utility;
using static IntegrationServices.Settings.ApplicationSettings;

namespace IntegrationServices.Providers
{
    public class JSONController : Controller
    {
        public JsonResult getJson(object obj)
        {

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        DAUser objDAUser = null;
        #region[GrantResourceOwnerCredentials]
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(context.UserName))
                {
                    HeaderErrorResponse(context, string.Format("User name {0}", StatusMessage.notExist));
                    return;
                }

                var password = context.Password;

                objDAUser = new DAUser();
                var dicRequestParams = getAllBodyParameters(context);
                string IP = dicRequestParams.ContainsKey("ClientIP") ? dicRequestParams["ClientIP"] : "";
                string ExpireTime = dicRequestParams.ContainsKey("ExpireTime") ? dicRequestParams["ExpireTime"] : "";
                string Source = dicRequestParams.ContainsKey("Source") ? dicRequestParams["Source"] : "";
                if (string.IsNullOrEmpty(password))
                {
                    HeaderErrorResponse(context, string.Format("Password {0}", StatusMessage.notExist));
                    return;
                }
                if (string.IsNullOrEmpty(Source))
                {
                    HeaderErrorResponse(context, string.Format("Source {0}", StatusMessage.notExist));
                    return;
                }

                if (string.IsNullOrEmpty(IP) && Is_Clientip_Required == 1)
                {
                    HeaderErrorResponse(context, string.Format("Client IP {0}", StatusMessage.notExist));
                    return;
                }

                try
                {
                    var userName = MiscHelper.DecodeTo64(context.UserName);
                    var consumerDetails = new BLAPIConsumerMaster().ValidateIntegrationServiceConsumer(userName, password, Source);
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                    if (!string.IsNullOrEmpty(ExpireTime))
                    {
                        context.Options.AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(Convert.ToDouble(ExpireTime));
                    }
                    if (consumerDetails != null)
                    {

                        context = GenerateToken(context, consumerDetails);

                    }
                    else
                    {
                        HeaderErrorResponse(context, string.Format("User {0}", StatusMessage.notExist));
                    }
                }
                catch (Exception ex)
                {
                    HeaderErrorResponse(context, string.Format("Username {0}", "is not valid !!"));
                }


            });

        }
        #endregion
        #region[ValidateClientAuthentication]
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }
        #endregion

        #region[GenerateToken]
        public OAuthGrantResourceOwnerCredentialsContext GenerateToken(OAuthGrantResourceOwnerCredentialsContext context, Models.Admin.APIConsumerMaster user)
        {
            try
            {
                var claims = new List<Claim>();
                IDictionary<string, string> dicResponseParams = new Dictionary<string, string>();

                var dicRequestParams = getAllBodyParameters(context);
                dicResponseParams.Add("userName", context.UserName);
                dicResponseParams.Add("user_id", user.consumer_id.ToString());
                dicResponseParams.Add("source_ref_type", user.source);
                context.OwinContext.Set<string>("username", context.UserName);
                claims = new List<Claim>() {
                    new Claim("username", context.UserName),
                      new Claim("user_id", user.consumer_id.ToString()),
                      new Claim("source_ref_type", "Web")
                     };

                ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties(dicResponseParams));

                context.Validated(ticket);

            }
            catch (KeyNotFoundException)
            {
                //HeaderErrorResponse(context, "Some parameters are missing in request!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return context;

        }
        #region[TokenEndpoint]
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            context.AdditionalResponseParameters.Add("status", StatusCodes.OK.ToString());

            context.AdditionalResponseParameters.Add("error_message", "");
            context.AdditionalResponseParameters.Add("result", null);
            context.AdditionalResponseParameters.Remove("token_type");
            return Task.FromResult<object>(null);

        }

        #endregion
        private static Dictionary<string, string> getAllBodyParameters(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var dicParams = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            var formCollectionTask = context.Request.ReadFormAsync();
            formCollectionTask.Wait();

            foreach (var pair in formCollectionTask.Result)
            {
                var value = pair.Value != null ? string.Join(",", pair.Value) : null;
                dicParams.Add(pair.Key, value);
            }
            return dicParams;
        }
        #endregion
        public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
        {
            private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

            public async Task CreateAsync(AuthenticationTokenCreateContext context)
            {
                try
                {
                    int loginId = Convert.ToInt32(context.OwinContext.Get<string>("loginId"));
                    int userId = Convert.ToInt32(context.OwinContext.Get<string>("userId"));
                    var refreshToken = Guid.NewGuid().ToString();
                    //new BLUserLogin().UpdateRefreshToken(userId, loginId, refreshToken);
                    // maybe only create a handle the first time, then re-use
                    _refreshTokens.TryAdd(refreshToken, context.Ticket);
                    // consider storing only the hash of the handle
                    context.SetToken(refreshToken);
                }
                catch (Exception)
                {

                }
            }

            public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
            {
                try
                {
                    var clientip = context.Request.RemoteIpAddress;
                    AuthenticationTicket ticket;
                    if (_refreshTokens.TryRemove(context.Token, out ticket))
                    {
                        context.SetTicket(ticket);
                    }
                }
                catch (Exception)
                {

                }
            }

            public void Create(AuthenticationTokenCreateContext context)
            {
                throw new NotImplementedException();
            }

            public void Receive(AuthenticationTokenReceiveContext context)
            {
                throw new NotImplementedException();
            }
        }
        #region[HeaderErrorResponse]
        public Models.API.ApiResponse<object> HeaderErrorResponse(OAuthGrantResourceOwnerCredentialsContext context, string sErrorMsg)
        {
            var resp = new Models.API.ApiResponse<object>();
            resp.error_message = sErrorMsg;
            resp.status = StatusCodes.REQUEST_DENIED.ToString();

            string jsonString = JsonConvert.SerializeObject(resp);
            context.SetError(new string(' ', jsonString.Length - 12));

            context.Response.StatusCode = 400;
            context.Response.Write(jsonString);

            return resp;
        }
        #endregion
    }
}