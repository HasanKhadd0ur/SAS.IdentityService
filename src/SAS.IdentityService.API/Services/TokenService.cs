using Ardalis.Result;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SAS.IdentityService.API.Abstraction;
using SAS.IdentityService.API.Entities;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;

namespace SAS.IdentityService.API.Services
{
    public class TokenService : ITokenService
    {

        private readonly JwtSetting _jwtSetting;
        public TokenService(IOptions<JwtSetting> jwtOptions)
        {
            _jwtSetting = jwtOptions.Value;
        }

        public Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            return Task.FromResult(GenerateToken(user));
        }

        public string GenerateToken(ApplicationUser user)
        {
            var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret)),
                    SecurityAlgorithms.HmacSha256
                );

            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),

            };

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience:_jwtSetting.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSetting.ExpireMinutes),
                claims : claims,
                signingCredentials:signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public Task<Result<string>> ValidateAndRenewTokenAsync(string refreshToken)
        {
            return Task.FromResult(Result.Success(refreshToken));
        }
    }
}
