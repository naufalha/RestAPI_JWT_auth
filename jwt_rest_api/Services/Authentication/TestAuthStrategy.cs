using System;
using System.Threading.Tasks;
using jwt_rest_api.Common;

namespace jwt_rest_api.Services.Authentication;

public class TestAuthStrategy : IAuthStrategy
{
    public Task<Result<StrategyUserPayload>> AuthenticateAsync(string credential)
    {
        if (string.IsNullOrWhiteSpace(credential))
        {
            return Task.FromResult(Result<StrategyUserPayload>.ValidationError("Test identity details are empty. Use format 'email:nickname'"));
        }

        try
        {
            string email;
            string name;

            var parts = credential.Split(':', 2);
            if (parts.Length == 2)
            {
                email = parts[0].Trim();
                name = parts[1].Trim();
            }
            else
            {
                email = credential.Trim();
                name = email.Split('@')[0]; // fallback nickname
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                return Task.FromResult(Result<StrategyUserPayload>.ValidationError("Invalid email address for test authentication."));
            }

            // For test logins, create a deterministic SubjectId based on the email
            var bytes = System.Text.Encoding.UTF8.GetBytes(email);
            var mockSubjectId = $"test-sub-{Convert.ToBase64String(bytes).Replace("=", "")}";

            return Task.FromResult(Result<StrategyUserPayload>.Success(new StrategyUserPayload
            {
                Email = email,
                Name = name,
                SubjectId = mockSubjectId
            }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<StrategyUserPayload>.Failure(ResultType.Failure, $"Test authentication failed: {ex.Message}"));
        }
    }
}
