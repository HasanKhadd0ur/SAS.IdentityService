using FluentAssertions;
using SAS.IdentityService.ApplicationCore.DTOs.Requests;
using SAS.IdentityService.Tests.IntegrationTests.Base;

namespace SAS.IdentityService.Tests.IntegrationTests.Services.Authentication
{
    public class AuthenticationServiceIntegrationTests : TestBase
    {
        [Fact]
        public async Task Register_Should_Create_User_And_Return_Token()
        {
            var service = CreateAuthenticationService();

            var request = new RegisterRequest
            {
                UserName = "integrationTestUser",
                Password = "Password@123",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var result = await service.Register(request);

            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().NotBeNullOrEmpty();
            result.Value.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
        {
            var service = CreateAuthenticationService();

            var registerRequest = new RegisterRequest
            {
                UserName = "loginTestUser",
                Password = "Password@123",
                Email = "login@example.com",
                FirstName = "Login",
                LastName = "User"
            };

            await service.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                UserName = "loginTestUser",
                Password = "Password@123"
            };

            var result = await service.Login(loginRequest);

            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().NotBeNullOrEmpty();
        }
    }
}

