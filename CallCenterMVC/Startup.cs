using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CallCenterMVC.Startup))]
namespace CallCenterMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
