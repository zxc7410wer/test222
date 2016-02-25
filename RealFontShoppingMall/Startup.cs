using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RealFontShoppingMall.Startup))]
namespace RealFontShoppingMall
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
