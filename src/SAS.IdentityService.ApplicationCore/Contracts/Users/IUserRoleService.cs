using Ardalis.Result;

namespace SAS.IdentityService.ApplicationCore.Contracts.Users
{
    public interface IUserRoleService
    {
        Task<bool> IsInRoleAsync(int userId, string roleName);
        Task<Result<List<string>>> GetUserRolesAsync(string email);
        Task<Result> AssignUserToRole(string email, string roleName);
        Task<Result> RemoveUserFromRole(string email, string roleName);
    }

}
