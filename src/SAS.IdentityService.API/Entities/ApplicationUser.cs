using Microsoft.AspNetCore.Identity;

namespace SAS.IdentityService.API.Entities;
public class ApplicationUser : IdentityUser<Guid>
{
    public String FirstName { get; set; }

    public String LastName { get; set; }
}