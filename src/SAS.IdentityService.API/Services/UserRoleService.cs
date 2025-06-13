using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using SAS.IdentityService.API.Abstraction;
using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRoleService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null && await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<Result<List<string>>> GetUserRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success(roles.ToList());
        }

        public async Task<Result> AssignUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.NotFound("User not found");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded ? Result.Success() :
                Result.Invalid(result.Errors.Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description }));
        }

        public async Task<Result> RemoveUserFromRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.NotFound("User not found");

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded ? Result.Success() :
                Result.Invalid(result.Errors.Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description }));
        }
    }
}
