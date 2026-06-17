using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using jwt_rest_api.Common;

namespace jwt_rest_api.Services.Authentication;

public class GoogleAuthStrategy : IAuthStrategy
{
    private readonly string _clientId;

    public GoogleAuthStrategy(IConfiguration configuration)
    {
        _clientId = configuration["GoogleAuth:ClientId"] 
            ?? throw new InvalidOperationException("Google Client ID is not configured in appsettings.json.");
    }

    public async Task<Result<StrategyUserPayload>> AuthenticateAsync(string credential)
    {
        if (string.IsNullOrWhiteSpace(credential))
        {
            return Result<StrategyUserPayload>.ValidationError("Google ID Token is empty.");
        }

        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            // Validate the token using Google API client library
            var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

            if (payload == null)
            {
                return Result<StrategyUserPayload>.Unauthorized("Invalid Google ID Token.");
            }

            return Result<StrategyUserPayload>.Success(new StrategyUserPayload
            {
                Email = payload.Email,
                Name = payload.Name ?? payload.Email,
                SubjectId = payload.Subject // The unique user ID on Google's side
            });
        }
        catch (InvalidJwtException ex)
        {
            return Result<StrategyUserPayload>.Unauthorized($"Google validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<StrategyUserPayload>.Failure(ResultType.Failure, $"An error occurred during Google authentication: {ex.Message}");
        }
    }
}
