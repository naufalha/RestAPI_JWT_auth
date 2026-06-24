using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using jwt_rest_api.Services;

namespace jwt_rest_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : BaseApiController
{
    private readonly IGameService _gameService;

    public AdminController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _gameService.GetAdminDashboardDataAsync();
        if (!result.IsSuccess)
        {
            return HandleResult(result);
        }

        return Ok(result.Value);
    }
}
