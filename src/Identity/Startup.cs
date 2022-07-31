using Application;
using AspNetCoreSpa.Infrastructure;
using AspNetCoreSpa.STS.Seed;
using ClassLibrary1.Identity.Migrations;
using Common;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCoreSpa.STS
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IProfileService, CustomProfileService>();
            services.AddTransient<IIdentitySeedData, IdentitySeedData>();

            services
                .AddApplication()
                .AddInfrastructure(Configuration, HostingEnvironment)
                .AddHealthChecks();
            
            
            services.AddStsServer(Configuration, HostingEnvironment);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseInfrastructure(HostingEnvironment);
            app.UseRouting();
            app.UseCors(Constants.DefaultCorsPolicy);
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
