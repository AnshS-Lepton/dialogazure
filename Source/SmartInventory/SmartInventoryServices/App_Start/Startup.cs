using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SmartInventoryServices.Providers;
using System;
using System.Web.Http;
namespace SmartInventoryServices
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new OAuthProvider(),
               // AccessTokenExpireTimeSpan = TimeSpan.FromDays(1), 
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(SmartInventory.Settings.ApplicationSettings.mobileAppLogoutTimeInSec),
                AllowInsecureHttp = true,
                RefreshTokenProvider = new SmartInventoryServices.Providers.OAuthProvider.SimpleRefreshTokenProvider()
            };
        }
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthBearerTokens(OAuthOptions);
        }
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           // app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}