using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SAS.IdentityService.ApplicationCore.Entities;

namespace SAS.IdentityService.Tests.IntegrationTests.Base
{
    public static class IdentityMocks
        {
            public static UserManager<ApplicationUser> CreateUserManager(IUserStore<ApplicationUser> store)
            {
                return new UserManager<ApplicationUser>(
                    store,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new PasswordHasher<ApplicationUser>(),
                    new IUserValidator<ApplicationUser>[0],
                    new IPasswordValidator<ApplicationUser>[0],
                    new UpperInvariantLookupNormalizer(),
                    new IdentityErrorDescriber(),
                    null,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
            }

            public static SignInManager<ApplicationUser> CreateSignInManager(UserManager<ApplicationUser> userManager)
            {
                return new SignInManager<ApplicationUser>(
                    userManager,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                    new Mock<IAuthenticationSchemeProvider>().Object,
                    new Mock<IUserConfirmation<ApplicationUser>>().Object);
            }
        }
    }
