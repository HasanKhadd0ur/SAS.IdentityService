using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SAS.IdentityService.ApplicationCore.Contracts.Tokens;
using SAS.IdentityService.ApplicationCore.Entities;
using SAS.IdentityService.Infrastructure.Persistence.DataContext;
using SAS.IdentityService.Infrastructure.Services.Authentication;
using SAS.IdentityService.Infrastructure.Services.Tokens;
using SAS.IdentityService.Tests.IntegrationTests.Config;

namespace SAS.IdentityService.Tests.IntegrationTests.Base
{
    public abstract class TestBase
    {
        protected readonly AppDbContext _dbContext;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly ITokenService _tokenService;

        protected TestBase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);

            var store = new UserStore<ApplicationUser, IdentityRole<Guid>, AppDbContext, Guid>(_dbContext);
            _userManager = IdentityMocks.CreateUserManager(store);
            _signInManager = IdentityMocks.CreateSignInManager(_userManager);
            _tokenService = new TokenService(Options.Create(TestJwtSettings.Get()));
        }

        protected AuthenticationService CreateAuthenticationService()
        {
            return new AuthenticationService(_userManager, _signInManager, _tokenService, _tokenService);
        }
    }
}
