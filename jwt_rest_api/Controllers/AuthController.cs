using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using jwt_rest_api.Models.Dto;
using jwt_rest_api.Services;
using jwt_rest_api.Services.Authentication;
using jwt_rest_api.Common;

namespace jwt_rest_api.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IAuthStrategyFactory _strategyFactory;
    private readonly ITokenService _tokenService;
    private readonly IGameService _gameService;

    public AuthController(
        IAuthStrategyFactory strategyFactory,
        ITokenService tokenService,
        IGameService gameService)
    {
        _strategyFactory = strategyFactory;
        _tokenService = tokenService;
        _gameService = gameService;
    }

    [HttpPost("login/google")]
    public async Task<IActionResult> LoginGoogle([FromBody] GoogleAuthRequest request)
    {
        try
        {
            var strategy = _strategyFactory.GetStrategy("google");
            var authResult = await strategy.AuthenticateAsync(request.IdToken);

            if (!authResult.IsSuccess)
            {
                return HandleResult(authResult);
            }

            var userPayload = authResult.Value!;
            var userResult = await _gameService.GetOrCreateUserAsync(userPayload);

            if (!userResult.IsSuccess)
            {
                return HandleResult(userResult);
            }

            var user = userResult.Value!;
            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Google login failed: {ex.Message}" });
        }
    }

    [HttpPost("login/test")]
    public async Task<IActionResult> LoginTest([FromBody] TestAuthRequest request)
    {
        try
        {
            var strategy = _strategyFactory.GetStrategy("test");
            
            // Format test payload as "email:nickname"
            var credential = $"{request.Email}:{request.Name}";
            var authResult = await strategy.AuthenticateAsync(credential);

            if (!authResult.IsSuccess)
            {
                return HandleResult(authResult);
            }

            var userPayload = authResult.Value!;
            var userResult = await _gameService.GetOrCreateUserAsync(userPayload);

            if (!userResult.IsSuccess)
            {
                return HandleResult(userResult);
            }

            var user = userResult.Value!;
            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Test login failed: {ex.Message}" });
        }
    }
}
