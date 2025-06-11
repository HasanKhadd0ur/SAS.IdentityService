using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Abstraction
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(ApplicationUser user);
    }
}
