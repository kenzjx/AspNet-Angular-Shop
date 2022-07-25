using System.Runtime.CompilerServices;
using Application.Abtractions;
using Application.Extensions;
using Application.Settings;
using ClassLibrary1.Email;
using ClassLibrary1.Enviroment;
using ClassLibrary1.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary1;

public static class ServicesExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            // Lambda này xác định xem có cần sự đồng ý của người dùng đối với các cookie không, cho một yêu cầu nhất định hay không.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;

        });

        services.AddSingleton<IDeploymentEnvironment, DeploymentEnvironment>();
        services.AddTransient<MachineDateTime>();
        services.AddScoped<IUserManager, UserManagerService>();
        services.AddTransient<EmailService, EmailService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor().AddResponseCompression()
            .AddMemoryCache().AddHealthChecks();
        
        // chia se cau hinh apps
        services.AddCustomConfiguration(configuration).
    }

    private static IServiceCollection AddCustomConfiguration(this IServiceCollection services ,IConfiguration configuration)
    {
        // doc section config
        services.ConfigurePoco<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
        return services;
    }
}