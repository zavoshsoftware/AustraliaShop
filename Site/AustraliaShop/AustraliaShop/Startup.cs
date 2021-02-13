using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AustraliaShop.Startup))]
namespace AustraliaShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
