using Domain.Entities.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Abtractions;

public interface ILocalizationDbContext
{
    DbSet<Culture> Cultures { get; set; }
    
    DbSet<Resource> Resources { get; set; }
    
    DatabaseFacade Database { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}