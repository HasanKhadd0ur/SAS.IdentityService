using Ardalis.Result;
using Microsoft.AspNetCore.Authentication;
using SAS.IdentityService.ApplicationCore.DTOs.Requests;
using SAS.IdentityService.ApplicationCore.DTOs.Response;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SAS.IdentityService.ApplicationCore.Contracts.Authentication
{
    public interface IAuthenticationService
    {
        public Task<Result<AuthenticationResponse>> Login(LoginRequest request);
        public Task<Result<AuthenticationResponse>> Register(RegisterRequest request);
        Task<Result<string>> RefreshTokenAsync(string refreshToken);
        Task<Result> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<Result<AuthenticationResponse>> ExternalLoginAsync();
        Task<AuthenticationProperties> ConfigureExternalAuthenticationProperties(string? provider, [StringSyntax(StringSyntaxAttribute.Uri)] string? redirectUrl, string? userId = null);

    }

}
