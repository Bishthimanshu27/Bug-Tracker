using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bug_tracker.Startup))]
namespace Bug_tracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
