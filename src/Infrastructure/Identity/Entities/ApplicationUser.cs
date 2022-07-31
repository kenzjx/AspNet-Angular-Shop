using Microsoft.AspNetCore.Identity;

namespace ClassLibrary1.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    
    public string Mobile { get; set; }
}