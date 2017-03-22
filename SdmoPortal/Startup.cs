using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SdmoPortal.Startup))]
namespace SdmoPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
