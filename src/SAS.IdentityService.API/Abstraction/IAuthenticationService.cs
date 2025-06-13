using Ardalis.Result;
using SAS.IdentityService.API.Models;
using System;
using System.Threading.Tasks;

namespace SAS.IdentityService.API.Abstraction
{
    public interface IAuthenticationService
    {
        public Task<Result<AuthenticationResult>> Login(LoginRequest request);
        public Task<Result<AuthenticationResult>> Register(RegisterRequest request);
        Task<Result<string>> RefreshTokenAsync(string refreshToken);
        Task<Result> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword);



    }

}
