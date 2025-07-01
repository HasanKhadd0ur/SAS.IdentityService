using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAS.IdentityService.ApplicationCore.Contracts.Authentication;
using SAS.IdentityService.ApplicationCore.DTOs.Requests;
using SAS.IdentityService.ApplicationCore.Entities;
using System.Security.Claims;

namespace SAS.IdentityService.API.Controllers
{
    public class AuthController : ApiBaseController
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request);
            return HandleResult(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.Register(request);
            return HandleResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _authService.UpdatePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("external-login")]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = "/")
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });
            var properties = await _authService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        [HttpGet("signin-google")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var result = await _authService.ExternalLoginAsync();

            if (result.IsSuccess)
            {
                // You can redirect with token if you want to pass it to frontend
                // OR just return JSON if you're building a SPA
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

    }
}