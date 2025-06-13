using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using SAS.IdentityService.API.Abstraction;
using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<ApplicationUser>>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();

            if (!users.Any())
            {
                return Result.NotFound("No users found.");
            }

            return Result.Success(users.AsEnumerable());
        }

        public async Task<Result<ApplicationUser>> GetUserByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return Result.NotFound($"User with ID {id} not found.");
            }

            return Result.Success(user);
        }
    }
}
