using System;
using System.Collections.Generic;
using System.Linq;

namespace jwt_rest_api.Services.Authentication;

public class AuthStrategyFactory : IAuthStrategyFactory
{
    private readonly IEnumerable<IAuthStrategy> _strategies;

    public AuthStrategyFactory(IEnumerable<IAuthStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IAuthStrategy GetStrategy(string provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Provider cannot be empty.", nameof(provider));
        }

        var normalizedProvider = provider.ToLowerInvariant();

        return normalizedProvider switch
        {
            "google" => _strategies.OfType<GoogleAuthStrategy>().FirstOrDefault() 
                ?? throw new InvalidOperationException("GoogleAuthStrategy is not registered."),
            "test" => _strategies.OfType<TestAuthStrategy>().FirstOrDefault() 
                ?? throw new InvalidOperationException("TestAuthStrategy is not registered."),
            "email" => _strategies.OfType<EmailAuthStrategy>().FirstOrDefault()
                ?? throw new InvalidOperationException("EmailAuthStrategy is not registered."),
            _ => throw new NotSupportedException($"Authentication provider '{provider}' is not supported.")
        };
    }
}
