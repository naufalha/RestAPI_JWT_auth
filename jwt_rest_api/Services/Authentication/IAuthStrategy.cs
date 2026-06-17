using System.Threading.Tasks;
using jwt_rest_api.Common;

namespace jwt_rest_api.Services.Authentication;

public interface IAuthStrategy
{
    Task<Result<StrategyUserPayload>> AuthenticateAsync(string credential);
}
