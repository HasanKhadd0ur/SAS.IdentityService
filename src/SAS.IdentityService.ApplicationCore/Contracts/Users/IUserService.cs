using Ardalis.Result;
using SAS.IdentityService.ApplicationCore.Entities;

namespace SAS.IdentityService.ApplicationCore.Contracts.Users
{
    public interface IUserService
    {
        Task<Result<IEnumerable<ApplicationUser>>> GetUsersAsync();
        Task<Result<ApplicationUser>> GetUserByIdAsync(Guid id);
    }

}
