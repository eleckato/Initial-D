using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(initial_d.Startup))]
namespace initial_d
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
