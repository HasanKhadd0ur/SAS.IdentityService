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

            if (result.Status == ResultStatus.NotFound)
            {
                return NotFound(result.Errors.FirstOrDefault() ?? "Not found.");
            }

            if (result.Status == ResultStatus.Invalid)
            {
                var firstError = result.ValidationErrors.FirstOrDefault();
                return BadRequest(new
                {
                    ErrorCode = firstError?.ErrorCode ?? "Invalid",
                    ErrorMessage = firstError?.ErrorMessage ?? "Validation failed."
                });
            }

            // General error (ResultStatus.Error)
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Errors = result.Errors
            });
        }

        // Optional for non-generic Result handling
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok();

            if (result.Status == ResultStatus.NotFound)
                return NotFound(result.Errors.FirstOrDefault() ?? "Not found.");

            if (result.Status == ResultStatus.Invalid)
            {
                var firstError = result.ValidationErrors.FirstOrDefault();
                return BadRequest(new
                {
                    ErrorCode = firstError?.ErrorCode ?? "Invalid",
                    ErrorMessage = firstError?.ErrorMessage ?? "Validation failed."
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Errors = result.Errors
            });
        }
    }
}
