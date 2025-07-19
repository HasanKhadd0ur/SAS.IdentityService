using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using SAS.IdentityService.ApplicationCore.Entities;
using SAS.IdentityService.Infrastructure.Services.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SAS.IdentityService.Tests.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
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

        public TokenServiceTests()
        {
            var jwtSettings = new JwtSetting
            {
                PrivateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC1DxAbBK/DvXSQU7qpt4tfSCGoGNP7ebWGjYcvtpFEcUm0MLvuVpPamc3w3MsdV4s25RJzF5ZHqW8RJULukFmwh1m7RLhvnJLd3XE2lVYU2GKMKq56bqPwDUVzw3RpwQzqRRlHj6FQQbosUQlp0HTJHK4bIhLqwOVYvx07rt0nhhElrfCCgITnwZA8jnvWOg0lBsZYQd3PFWhioWPao1NFrVMXuLB+/3xgbp8BSczcYQdVx/at8jXL0+IFhBFtbIEg2uN3806qRjAnu4hahoHRKCgQKNs4j8wBRmWULARbnfavzkQGCFkFgia9CAuuVDQe1ZY5XnN6m2YGSTnm1W4bAgMBAAECggEAFr2c65J7yP1OBfqeN/F3rdtPbt9/cIG+4kwlrevOSbxjjMrk2nCWrXQilkgFCDBrVH+9AND2QmtDf0cg0E83v0ugt5FQC6SsxZ+p4o3V0VRu5uVNeM8rWVvcB0gwoj+/BtrwQTlcV3TfP0WxBaX1pR+BtqK29t6MxR1r5zJ1UHX43M8td5ZeFSa7csHSUj06wqFSk5CBi7DXZ7CpuJqFA1Jw1e7/uSi91zxfUFi2Gban0WsolzsJVLPq4tvUtCDV2KqzudH//p7ApUxSSVjjweTmt7Iv+4BibhKb5vDOw0U8bL0yY49GlGKkUhds/mpM2VzVBTvWm5iztueoysSYkQKBgQDZ4ZqIpsVqoduHQVVj82+8ErdfOlH72N0Tkq+K1A0KsFBFQHJYQQCBGJwktpI/enspgh7QsUOjsWpivN908npPrvRcXLL1BKobi4/Sdzum2Q/wUS1CAzxjZiSS8eiTU6W29vg2e3xqBSJiIO+eBKSjc8e+kE7tsOq4gk4wYISP7QKBgQDUvESuaY4Zj/R9Fe4bbfLckqED0OXqqfL785V/QCL0E9X39IO6NFHSycZfcKNn5zvtIEMt6n0/kfHq44CHAFdAGTR+y/QDn7gC0aJCKYXJsblyApaX7rdDv3I1XIN/VcNN55pDV7Jz38Np0H9hGmVXHycFpVfjcG6URia2Q+dlJwKBgBpzPONh+41aYOAmmksr/mCClShGWDWOuifqIs1juMGXbec6T6dPHNSPPEVotJBoOhNr7HnBS0jP/O5Hp4j29so9nwSnbEI6crwtUSRnvSIgL8Mo16mv4uDeQYplO6rG+NMhYDic/RmRzf7DqNMyFRqSr6j+s9Y/J/+GMh/FV9JxAoGAKWgLpjb9ZZRTbhVapxQbdTtFA80hMy+pD8k1i/Mb2o4R05VQHmoYkwKbDfCKnaqwS1NAQCun/TuW7rUhSyWR1fQR96wS7rGjy0iqYF5coAlrovlF/oSEPzeAL0q2fY5f2UX/DKSBI3SmQG5InMnad76khcDM/oPfwhR1bHyDCXkCgYBpN4G7TwkLtWRnrwCK8aNNsNWRLEkFH6kpNm80UdhEu54pu5HHc8nj9FZVcltcks3X7whb4CbEbzC1vDeD4pJtXk9hncmst4aD051goLpdWoPVk0R0TE4FarGzYfhSQ1r9+vnVxoAE9LeA2BsZpDcspuRQO6q0li/c+HN0o9ekdA==",
                //Secret = "ThisIsASecretKeyForJwtTesting123!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpireMinutes = 60
            };
            var options = Options.Create(jwtSettings);
            _tokenService = new TokenService(options,GetMockedUserManager());
        }

        [Fact]
        public async void GenerateToken_Returns_NonEmptyString()
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "testuser@example.com",
                FirstName = "Test"
            };

            string token = await _tokenService.GenerateToken(user);

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
