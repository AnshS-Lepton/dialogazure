using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using IntegrationServices.Providers;
using System;
using System.Web.Http;
namespace IntegrationServices
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/GenerateToken"),
                Provider = new OAuthProvider(),               
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(86400),               
                AllowInsecureHttp = true,                
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
