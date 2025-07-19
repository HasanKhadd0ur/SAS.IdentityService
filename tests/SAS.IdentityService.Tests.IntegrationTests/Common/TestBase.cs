using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
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

        public static UserManager<ApplicationUser> GetMockedUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Mock GetRolesAsync to return predefined roles
            mgr.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
               .ReturnsAsync(new List<string> { "Admin", "User" });

            return mgr.Object;
        }

        protected TestBase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);

            var store = new UserStore<ApplicationUser, IdentityRole<Guid>, AppDbContext, Guid>(_dbContext);
            _userManager = IdentityMocks.CreateUserManager(store);
            _signInManager = IdentityMocks.CreateSignInManager(_userManager);
            _tokenService = new TokenService(Options.Create(TestJwtSettings.Get()),_userManager);
        }

        protected AuthenticationService CreateAuthenticationService()
        {
            return new AuthenticationService(_userManager, _signInManager, _tokenService, _tokenService);
        }
    }
}
