using Ardalis.Result;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SAS.IdentityService.ApplicationCore.Contracts.Tokens;
using SAS.IdentityService.ApplicationCore.Entities;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SAS.IdentityService.Infrastructure.Services.Tokens
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
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(_jwtSetting.PrivateKey), out _);
            
            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256);

            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName)
            };

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSetting.ExpireMinutes),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public Task<Result<string>> ValidateAndRenewTokenAsync(string refreshToken)
        {
            return Task.FromResult(Result.Success(refreshToken));
        }
    }
}
