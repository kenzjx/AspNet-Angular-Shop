using ClassLibrary1.Identity.Entities;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClassLibrary1.Identity.Migrations;

public class IdentityServerDbContext :  IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
    ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
    ApplicationRoleClaim, ApplicationUserToken>, IPersistedGrantDbContext
{
    private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

    public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options)
    {
        _operationalStoreOptions = operationalStoreOptions;
    }

    public DbSet<PersistedGrant> PersistedGrants { get; set; }
    public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
    public DbSet<Key> Keys { get; set; }
    public DbSet<ServerSideSession> ServerSideSessions { get; set; }
    
    public DbSet<Client> Clients { set; get; }
    public DbSet<IdentityResource> IdentityResources { set; get; }
    public DbSet<ApiResource> ApiResources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        
    }
}