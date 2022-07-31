using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Application.Abtractions;
using Application.Extensions;
using Application.Settings;
using ClassLibrary1.Email;
using ClassLibrary1.Enviroment;
using ClassLibrary1.Identity.Entities;
using ClassLibrary1.Identity.Migrations;
using ClassLibrary1.Persistence;
using ClassLibrary1.Services;
using ClassLibrary1.Services.Certificate;
using Common;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.AspNetCore.Processors;
using NSwag.Generation.Processors.Security;

namespace AspNetCoreSpa.Infrastructure;

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
        services.AddTransient<IDateTime, MachineDateTime>();
        services.AddScoped<IUserManager, UserManagerService>();
        services.AddTransient<EmailService, EmailService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor().AddResponseCompression()
            .AddMemoryCache().AddHealthChecks();
        
        // chia se cau hinh apps
        services.AddCustomConfiguration(configuration).AddCustomSignalR()
            .AddCustomCors(configuration)
            .AddCustomUi(environment);

        return services;
    }

    public static IIdentityServerBuilder AddStsServer(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddTransient<IClientInfoService, ClientInfoService>();
        services.AddDbContext<IdentityServerDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("Identity"),
                b => b.MigrationsAssembly(typeof(IdentityServerDbContext).Assembly.FullName));

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                
            }
        });

        services.AddDefaultIdentity<ApplicationUser>().AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityServerDbContext>();

        // var x509Certificate2 = GetCertificate(environment, configuration);
        
        var identityBuilder = services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, IdentityServerDbContext>(
                options =>
                {
                    var corsList = configuration.GetSection("CorsOrigins").Get<List<string>>();
                    
                    corsList.ForEach(host =>
                    {
                        var uri = new Uri(host);
                        var hostElements = uri.AbsoluteUri.Replace(":", string.Empty)
                            .Replace("/", string.Empty).Split(".");

                        var clientId = string.Join(string.Empty, hostElements);

                        options.Clients.AddSPA(clientId, spa => spa
                            .WithRedirectUri($"{host}/authentication/login-callback")
                            .WithScopes(new string[] { clientId })
                            .WithLogoutRedirectUri($"{host}/authentication/logout-callback"));
                        
                        options.ApiResources.AddApiResource(clientId, resource =>
                        {
                            resource.WithScopes(clientId);
                        });

                    });
                });
        services.AddAuthentication()
            .AddIdentityServerJwt()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["IdentityServer:ExternalAuth:Google:ClientId"];
                options.ClientSecret = configuration["IdentityServer:ExternalAuth:Google:ClientSecret"];
            })
            .AddFacebook(options =>
            {
                options.AppId = configuration["IdentityServer:ExternalAuth:Facebook:AppId"];
                options.AppSecret = configuration["IdentityServer:ExternalAuth:Facebook:AppSecret"];
            });
        
        

        return identityBuilder;
    }

    public static IServiceCollection ADdPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext < ApplicationDbContext > (options =>
        {
            bool.TryParse(configuration["Data:useSqLite"], out var useSqlite);
            bool.TryParse(configuration["Data:useInMemory"], out var useInMemory);
            var connectionString = configuration["Data:Web"];
            
            
            if (useInMemory)
            {
                options.UseInMemoryDatabase(nameof(AspNetCoreSpa)); // Takes database name
            }
            else if (useSqlite)
            {
                options.UseSqlite(connectionString, b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    //b.UseNetTopologySuite();
                });
            }
            else
            {
                options.UseSqlServer(connectionString, b =>
                {
                    b.MigrationsAssembly("AspNetCoreSpa.Infrastructure");
                    // Add following package to enable net topology suite for sql server:
                    // Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite
                    //b.UseNetTopologySuite();
                });
            }
        });

        return services;
    }
    
    
    // private static X509Certificate2 GetCertificate(IWebHostEnvironment environment, IConfiguration configuration)
    // {
    //     var useDevCertificate = bool.Parse(configuration["UseDevCertificate"]);
    //
    //     var cert = new X509Certificate2(Path.Combine(environment.ContentRootPath, "sts_dev_cert.pfx"), "1234");
    //
    //     if (environment.IsProduction() && !useDevCertificate)
    //     {
    //         var useLocalCertStore = Convert.ToBoolean(configuration["UseLocalCertStore"]);
    //
    //         if (useLocalCertStore)
    //         {
    //             using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
    //             var certificateThumbprint = configuration["CertificateThumbprint"];
    //
    //             store.Open(OpenFlags.ReadOnly);
    //             var certs = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
    //             cert = certs[0];
    //             store.Close();
    //         }
    //         else
    //         {
    //             // Azure deployment, will be used if deployed to Azure
    //             var vaultConfigSection = configuration.GetSection("Vault");
    //             var keyVaultService = new KeyVaultCertificateService(vaultConfigSection["Url"], vaultConfigSection["ClientId"], vaultConfigSection["ClientSecret"]);
    //             cert = keyVaultService.GetCertificateFromKeyVault(vaultConfigSection["CertificateName"]);
    //         }
    //     }
    //     return cert;
    // }

    private static IServiceCollection AddCustomConfiguration(this IServiceCollection services ,IConfiguration configuration)
    {
        // doc section config
        services.ConfigurePoco<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
        return services;
    }

    private static IServiceCollection AddCustomSignalR(this IServiceCollection services)
    {
        // add SignalR
        services.AddSignalR()
            .AddMessagePackProtocol();

        return services;
    }

    private static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        // doc cau hinh json add cors
        services.AddCors(options =>
        {
            options.AddPolicy(Common.Constants.DefaultCorsPolicy, builder =>
                {
                    var conrsList = configuration.GetSection("CorsOrigins").Get<List<string>>()?.ToArray() ??
                                    new string[] { };
                    builder.WithOrigins(conrsList).AllowAnyMethod()
                        .AllowAnyHeader();

                })
                ;
        });
        return services;
    }

    private static IServiceCollection AddCustomUi(this IServiceCollection services, IWebHostEnvironment environment)
    {
        //Add swagger Api document
        
        services.AddOpenApiDocument(configure =>
        {
            configure.Title = "AspNetShop API";
            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });
            
            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

        });
        
        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        var controllerWithViews = services.AddControllersWithViews();
        var razorPages = services.AddRazorPages()
            .AddViewLocalization().AddDataAnnotationsLocalization();
        
        if(environment.IsDevelopment())
        {
            controllerWithViews.AddRazorRuntimeCompilation();
            razorPages.AddRazorRuntimeCompilation();
        }

        return services;
    }
}