using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartFeasibility.Startup))]
namespace SmartFeasibility
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
        }
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
