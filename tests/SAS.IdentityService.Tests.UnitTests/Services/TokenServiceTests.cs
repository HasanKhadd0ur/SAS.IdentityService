using FluentAssertions;
using Microsoft.Extensions.Options;
using SAS.IdentityService.ApplicationCore.Entities;
using SAS.IdentityService.Infrastructure.Services.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SAS.IdentityService.Tests.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            var jwtSettings = new JwtSetting
            {
                Secret = "ThisIsASecretKeyForJwtTesting123!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpireMinutes = 60
            };
            var options = Options.Create(jwtSettings);
            _tokenService = new TokenService(options);
        }

        [Fact]
        public void GenerateToken_Returns_NonEmptyString()
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "testuser@example.com",
                FirstName = "Test"
            };

            string token = _tokenService.GenerateToken(user);

            token.Should().NotBeNullOrEmpty();

            // Optional: validate token structure
            var handler = new JwtSecurityTokenHandler();
            handler.CanReadToken(token).Should().BeTrue();

            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Issuer.Should().Be("TestIssuer");
            jwtToken.Audiences.Should().Contain("TestAudience");
        }

        [Fact]
        public async Task GenerateRefreshTokenAsync_Returns_NonEmptyString()
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "testuser@example.com",   
                FirstName = "Test",
                LastName="Test"
            };

            string refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            refreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ValidateAndRenewTokenAsync_ReturnsSuccessResult_WithSameToken()
        {
            string testToken = "test-refresh-token";

            var result = await _tokenService.ValidateAndRenewTokenAsync(testToken);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(testToken);
        }
    }
}
