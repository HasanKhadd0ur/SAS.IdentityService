using Ardalis.Result;
using SAS.IdentityService.ApplicationCore.Entities;

namespace SAS.IdentityService.ApplicationCore.Contracts.Roles
{
    public interface IRoleService
    {
        Task<Result> CreateRoleAsync(string roleName);
        Task<Result> DeleteRoleAsync(int roleId);
        Task<List<Role>> GetRolesAsync();
        Task<Result<Role>> GetRoleByIdAsync(int id);
        Task<Result<Role>> UpdateRole(int id, string roleName);
    }

}
