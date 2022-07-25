using Microsoft.AspNetCore.Identity;

namespace ClassLibrary1.Identity.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; }
    
   
}