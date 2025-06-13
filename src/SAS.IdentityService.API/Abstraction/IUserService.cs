using Ardalis.Result;
using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Abstraction
{
    public interface IUserService
    {
        Task<Result<IEnumerable<ApplicationUser>>> GetUsersAsync();
        Task<Result<ApplicationUser>> GetUserByIdAsync(Guid id);
    }

}
