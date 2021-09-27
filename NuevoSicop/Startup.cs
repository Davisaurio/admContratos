using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NuevoSicop.Startup))]
namespace NuevoSicop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
