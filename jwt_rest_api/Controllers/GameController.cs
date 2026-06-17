using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using jwt_rest_api.Models.Dto;
using jwt_rest_api.Services;

namespace jwt_rest_api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class GameController : BaseApiController
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("progress")]
    public async Task<IActionResult> GetProgress()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User identifier not found in token." });
        }

        var result = await _gameService.GetProgressAsync(userId);
        if (!result.IsSuccess)
        {
            return HandleResult(result);
        }

        var progress = result.Value!;
        
        // Parse the inventory and state data strings back to JSON structures for the client
        var inventory = JsonSerializer.Deserialize<System.Collections.Generic.List<string>>(progress.InventoryJson) 
                        ?? new System.Collections.Generic.List<string>();
        var stateData = JsonSerializer.Deserialize<JsonElement>(progress.StateDataJson);

        return Ok(new GameProgressDto
        {
            Level = progress.Level,
            Score = progress.Score,
            Coins = progress.Coins,
            Inventory = inventory,
            StateData = stateData
        });
    }

    [HttpPost("progress")]
    public async Task<IActionResult> SaveProgress([FromBody] GameProgressDto request)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User identifier not found in token." });
        }

        var result = await _gameService.SaveProgressAsync(userId, request);
        return HandleResult(result);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
    }
}
