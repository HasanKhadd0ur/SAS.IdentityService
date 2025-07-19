using Ardalis.Result;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SAS.IdentityService.ApplicationCore.Contracts.Tokens;
using SAS.IdentityService.ApplicationCore.DTOs.Requests;
using SAS.IdentityService.ApplicationCore.Entities;
using SAS.IdentityService.Infrastructure.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using AuthenticationService = SAS.IdentityService.Infrastructure.Services.Authentication.AuthenticationService;

namespace SAS.IdentityService.Tests.UnitTests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly AuthenticationService _service;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signinManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        public AuthenticationServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var userConfirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            options.Setup(o => o.Value).Returns(new IdentityOptions());

            _signinManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                contextAccessor.Object,
                userClaimsPrincipalFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                userConfirmation.Object);

            _tokenServiceMock = new Mock<ITokenService>();

            _service = new AuthenticationService(
                _userManagerMock.Object,
                _signinManagerMock.Object,
                _tokenServiceMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ReturnsResponse_WhenUserFound()
        {
            var user = new ApplicationUser { UserName = "test" };

            _userManagerMock.Setup(x => x.FindByNameAsync("test")).ReturnsAsync(user);
            _signinManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "pass", false)).ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns(Task.FromResult("access-token"));
            _tokenServiceMock.Setup(x => x.GenerateRefreshTokenAsync(user)).ReturnsAsync("refresh-token");
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            var result = await _service.Login(new LoginRequest { UserName = "test", Password = "pass" });

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().Be("access-token");
            result.Value.TokenInfo.RefreshToken.Should().Be("refresh-token");
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccess_WhenUserCreated()
        {
            var registerRequest = new RegisterRequest
            {
                UserName = "newuser",
                Email = "newuser@test.com",
                FirstName = "First",
                LastName = "Last",
                Password = "Password123!"
            };

            var user = new ApplicationUser
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Success);

            _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>())).ReturnsAsync("access-token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("refresh-token");
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());

            var result = await _service.Register(registerRequest);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().Be("access-token");
            result.Value.TokenInfo.RefreshToken.Should().Be("refresh-token");
        }

        [Fact]
        public async Task RegisterAsync_ReturnsInvalid_WhenCreationFails()
        {
            var registerRequest = new RegisterRequest
            {
                UserName = "failuser",
                Email = "failuser@test.com",
                FirstName = "First",
                LastName = "Last",
                Password = "Password123!"
            };

            var identityErrors = new List<IdentityError> {
                new IdentityError { Code = "DuplicateUserName", Description = "User already exists." }
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var result = await _service.Register(registerRequest);

            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("User already exists.");
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsTokenResult()
        {
            var refreshToken = "some-refresh-token";
            var expectedResult = Result.Success("new-access-token");

            _tokenServiceMock.Setup(x => x.ValidateAndRenewTokenAsync(refreshToken))
                .ReturnsAsync(expectedResult);

            var result = await _service.RefreshTokenAsync(refreshToken);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ReturnsSuccess_WhenPasswordChanged()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "oldPass", "newPass"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _service.UpdatePasswordAsync(userId, "oldPass", "newPass");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePasswordAsync_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

            var result = await _service.UpdatePasswordAsync(userId, "oldPass", "newPass");

            result.Status.Should().Be(Ardalis.Result.ResultStatus.NotFound);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ReturnsInvalid_WhenChangeFails()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser();

            var identityErrors = new List<IdentityError> {
                new IdentityError { Code = "PasswordMismatch", Description = "Incorrect password." }
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "oldPass", "newPass"))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var result = await _service.UpdatePasswordAsync(userId, "oldPass", "newPass");

            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("Incorrect password.");
        }

        [Fact]
        public async Task ExternalLoginAsync_ReturnsError_WhenExternalInfoIsNull()
        {
            _signinManagerMock
                .Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync((ExternalLoginInfo?)null);

            var result = await _service.ExternalLoginAsync();

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e == "Failed to load external login info.");
        }
        [Fact]
        public async Task ExternalLoginAsync_ReturnsSuccess_WhenLoginSucceeds()
        {
            var loginInfo = new ExternalLoginInfo(
                new ClaimsPrincipal(),
                "provider",
                "providerKey",
                "displayName");

            var user = new ApplicationUser { UserName = "extuser" };

            // Return valid external login info
            _signinManagerMock
                .Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(loginInfo);

            _signinManagerMock.Setup(x => x.ExternalLoginSignInAsync("provider", "providerKey", false, true))
                .ReturnsAsync(SignInResult.Success);

            _userManagerMock.Setup(x => x.FindByLoginAsync("provider", "providerKey")).ReturnsAsync(user);

            _tokenServiceMock.Setup(x => x.GenerateToken(user)).ReturnsAsync("access-token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshTokenAsync(user)).ReturnsAsync("refresh-token");
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            var result = await _service.ExternalLoginAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().Be("access-token");
        }

        [Fact]
        public async Task ExternalLoginAsync_ReturnsInvalid_WhenUserCreationFails()
        {
            var externalLoginInfo = new ExternalLoginInfo(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Email, "test@example.com")
                }, "TestAuthType")),
                "TestProvider",
                "ProviderKey",
                "DisplayName");

            _signinManagerMock
                .Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(externalLoginInfo);

            _signinManagerMock.Setup(x => x.ExternalLoginSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), false, true))
                .ReturnsAsync(SignInResult.Failed);

            _userManagerMock.Setup(x => x.FindByEmailAsync("test@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = "Username already exists"
                }));

            var result = await _service.ExternalLoginAsync();

            result.IsSuccess.Should().BeFalse();
            result.ValidationErrors.Should().ContainSingle();
            result.ValidationErrors.First().Identifier.Should().Be("DuplicateUserName");
            result.ValidationErrors.First().ErrorMessage.Should().Be("Username already exists");
        }

        [Fact]
        public async Task ConfigureExternalAuthenticationProperties_ReturnsProperties()
        {
            var authProperties = new AuthenticationProperties();
            _signinManagerMock.Setup(x => x.ConfigureExternalAuthenticationProperties("provider", "url", null))
                .Returns(authProperties);

            var result = await _service.ConfigureExternalAuthenticationProperties("provider", "url");

            result.Should().Be(authProperties);
        }
    }
}