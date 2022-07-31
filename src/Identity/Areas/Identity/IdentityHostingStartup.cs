using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(AspNetCoreSpa.STS.Areas.Identity.IdentityHostingStartup))]
namespace AspNetCoreSpa.STS.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}