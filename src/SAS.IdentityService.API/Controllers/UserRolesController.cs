using Microsoft.AspNetCore.Mvc;
using SAS.IdentityService.ApplicationCore.Contracts.Users;

namespace SAS.IdentityService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ApiBaseController
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet("is-in-role")]
        public async Task<IActionResult> IsInRole([FromQuery] int userId, [FromQuery] string roleName)
        {
            var result = await _userRoleService.IsInRoleAsync(userId, roleName);
            return Ok(result);
        }

        [HttpGet("{email}/roles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var result = await _userRoleService.GetUserRolesAsync(email);
            return HandleResult(result);
        }

        [HttpPost("{email}/assign-role")]
        public async Task<IActionResult> AssignRole(string email, [FromQuery] string roleName)
        {
            var result = await _userRoleService.AssignUserToRole(email, roleName);
            return HandleResult(result);
        }

        [HttpPost("{email}/remove-role")]
        public async Task<IActionResult> RemoveRole(string email, [FromQuery] string roleName)
        {
            var result = await _userRoleService.RemoveUserFromRole(email, roleName);
            return HandleResult(result);
        }
    }
}