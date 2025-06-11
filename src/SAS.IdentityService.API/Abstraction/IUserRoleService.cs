
namespace SAS.IdentityService.API.Abstraction
{
    public interface IUserRoleService
    {
        Task<bool> IsInRoleAsync(int userId, string roleName);
        Task<Result<List<string>>> GetUserRolesAsync(string email);
        Task<Result> AssignUserToRole(string email, string roleName);
        Task<Result> RemoveUserFromRole(string email, string roleName);
    }

}
