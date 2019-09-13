using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HappyHotels.Startup))]
namespace HappyHotels
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
