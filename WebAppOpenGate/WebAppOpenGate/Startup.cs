using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppOpenGate.Startup))]
namespace WebAppOpenGate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
