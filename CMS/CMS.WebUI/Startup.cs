using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMS.WebUI.Startup))]
namespace CMS.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}