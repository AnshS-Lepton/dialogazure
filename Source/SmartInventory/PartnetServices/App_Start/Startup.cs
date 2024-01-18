using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SmartInventory.Startup))]

namespace SmartInventory
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR();
            app.Map("/signalr", map =>
            {
                // map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration { EnableDetailedErrors = true, EnableJSONP = true };
                map.RunSignalR();
            });


            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
