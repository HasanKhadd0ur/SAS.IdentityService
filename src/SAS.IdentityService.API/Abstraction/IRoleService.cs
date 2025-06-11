
using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Abstraction
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
