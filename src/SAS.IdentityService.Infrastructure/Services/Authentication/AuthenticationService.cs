using Ardalis.Result;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using SAS.IdentityService.ApplicationCore.Contracts.Tokens;
using SAS.IdentityService.ApplicationCore.DTOs.Requests;
using SAS.IdentityService.ApplicationCore.DTOs.Response;
using SAS.IdentityService.ApplicationCore.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SAS.IdentityService.Infrastructure.Services.Authentication
{
    public class AuthenticationService : ApplicationCore.Contracts.Authentication.IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _jwtTokenGenerator;
        private readonly ITokenService _tokenService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService jwtTokenGenerator,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthenticationResponse>> Login(LoginRequest request)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return Result.Invalid(new ValidationError { Identifier = "Username", ErrorMessage = "Invalid credentials." });

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
                return Result.Invalid(new ValidationError { Identifier = "Password", ErrorMessage = "Invalid credentials." });

            var token = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            var authResult = new AuthenticationResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await _userManager.GetRolesAsync(user).ContinueWith(t => t.Result.Select(r => new Role { Name = r }).ToList()),
                Token = token,
                TokenInfo = new TokenInfo
                {
                    Username = user.UserName,
                    RefreshToken = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddDays(7) // Example expiration
                }
            };

            return Result.Success(authResult);
        }

        public async Task<Result<AuthenticationResponse>> Register(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors
                    .Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description })
                    .ToList();
                return Result.Invalid(errors);
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            var authResult = new AuthenticationResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await _userManager.GetRolesAsync(user).ContinueWith(t => t.Result.Select(r => new Role { Name = r }).ToList()),
                Token = token,
                TokenInfo = new TokenInfo
                {
                    Username = user.UserName,
                    RefreshToken = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddDays(7) // Example expiration
                }
            };

            return Result.Success(authResult);
        }

        public async Task<Result<string>> RefreshTokenAsync(string refreshToken)
        {
            var result = await _tokenService.ValidateAndRenewTokenAsync(refreshToken);
            return result;
        }

        public async Task<Result> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.NotFound("User not found.");

            var changeResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!changeResult.Succeeded)
            {
                var errors = changeResult.Errors
                    .Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description })
                    .ToList();
                return Result.Invalid(errors);
            }

            return Result.Success();
        }
        public async Task<Result<AuthenticationResponse>> ExternalLoginAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Result.Error("Failed to load external login info.");

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            ApplicationUser user;

            if (signInResult.Succeeded)
            {
                user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrWhiteSpace(email))
                    return Result.Error("Email not found from external provider.");

                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        var errors = createResult.Errors
                            .Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description })
                            .ToList();
                        return Result.Invalid(errors);
                    }
                }

                var loginResult = await _userManager.AddLoginAsync(user, info);
                if (!loginResult.Succeeded)
                {
                    var errors = loginResult.Errors
                        .Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description })
                        .ToList();
                    return Result.Invalid(errors);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            // Generate JWT and refresh token
            var token = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            var authResult = new AuthenticationResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await _userManager.GetRolesAsync(user).ContinueWith(t => t.Result.Select(r => new Role { Name = r }).ToList()),
                Token = token,
                TokenInfo = new TokenInfo
                {
                    Username = user.UserName,
                    RefreshToken = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddDays(7)
                }
            };

            return Result.Success(authResult);
        }

        public Task<AuthenticationProperties> ConfigureExternalAuthenticationProperties(string? provider, [StringSyntax(StringSyntaxAttribute.Uri)] string? redirectUrl, string? userId = null)
        {
            return Task.FromResult(_signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl));
        }
    }
}
