using jwt_rest_api.Models;

namespace jwt_rest_api.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
