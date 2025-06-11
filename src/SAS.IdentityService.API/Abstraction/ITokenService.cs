using Ardalis.Result;
using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Abstraction
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
        Task<Result<string>> ValidateAndRenewTokenAsync(string refreshToken);

    }
}
