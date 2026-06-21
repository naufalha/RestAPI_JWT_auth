using System;
using System.Threading.Tasks;
using jwt_rest_api.Common;

namespace jwt_rest_api.Services.Authentication;

// Simple example strategy that accepts email/password pairs for demo purposes.
// In a real system you'd validate against a user store, LDAP, or external service.
public class EmailAuthStrategy : IAuthStrategy
{
    public Task<Result<StrategyUserPayload>> AuthenticateAsync(string credential)
    {
        if (string.IsNullOrWhiteSpace(credential))
        {
            return Task.FromResult(Result<StrategyUserPayload>.ValidationError("Email credentials are empty. Use format 'email:password'."));
        }

        try
        {
            var parts = credential.Split(':', 2);
            if (parts.Length != 2)
            {
                return Task.FromResult(Result<StrategyUserPayload>.ValidationError("Invalid credential format. Use 'email:password'."));
            }

            var email = parts[0].Trim();
            var password = parts[1].Trim();

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                return Task.FromResult(Result<StrategyUserPayload>.ValidationError("Invalid email address."));
            }

            // Demo validation: accept any password equal to 'password123'
            if (password != "password123")
            {
                return Task.FromResult(Result<StrategyUserPayload>.Unauthorized("Invalid email or password."));
            }

            // Create deterministic subject id for this demo
            var bytes = System.Text.Encoding.UTF8.GetBytes(email);
            var subjectId = $"email-sub-{Convert.ToBase64String(bytes).Replace("=", "")}";

            return Task.FromResult(Result<StrategyUserPayload>.Success(new StrategyUserPayload
            {
                Email = email,
                Name = email.Split('@')[0],
                SubjectId = subjectId
            }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<StrategyUserPayload>.Failure(ResultType.Failure, $"Email authentication failed: {ex.Message}"));
        }
    }
}
