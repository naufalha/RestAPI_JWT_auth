using Microsoft.AspNetCore.Mvc;
using jwt_rest_api.Common;

namespace jwt_rest_api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return MapFailedResult(result);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return MapFailedResult(result);
    }

    private IActionResult MapFailedResult(Result result)
    {
        return result.Type switch
        {
            ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
            ResultType.ValidationError => BadRequest(new { error = result.ErrorMessage }),
            ResultType.Unauthorized => Unauthorized(new { error = result.ErrorMessage }),
            ResultType.Conflict => Conflict(new { error = result.ErrorMessage }),
            _ => StatusCode(500, new { error = result.ErrorMessage })

        };
    }
}
