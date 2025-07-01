using Microsoft.AspNetCore.Identity;

namespace SAS.IdentityService.ApplicationCore.Entities;
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}