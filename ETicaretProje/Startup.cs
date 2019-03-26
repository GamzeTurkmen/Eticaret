using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ETicaretProje.Startup))]
namespace ETicaretProje
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
