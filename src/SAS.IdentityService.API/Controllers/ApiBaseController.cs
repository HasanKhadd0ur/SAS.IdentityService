using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;

namespace SAS.IdentityService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return Problem(
                    detail: result.ValidationErrors.FirstOrDefault().ErrorMessage,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: result.ValidationErrors.FirstOrDefault().ErrorCode
                    );
            }
        }

    }
}