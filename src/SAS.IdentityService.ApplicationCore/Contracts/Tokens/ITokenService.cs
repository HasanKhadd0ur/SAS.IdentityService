using Ardalis.Result;
using SAS.IdentityService.ApplicationCore.Entities;

namespace SAS.IdentityService.ApplicationCore.Contracts.Tokens
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
        Task<Result<string>> ValidateAndRenewTokenAsync(string refreshToken);

    }
}
