using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>,Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }


    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<TokenInfo> TokenInfos { get; set; }
}
