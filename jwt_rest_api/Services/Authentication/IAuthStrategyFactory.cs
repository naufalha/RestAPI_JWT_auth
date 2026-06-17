namespace jwt_rest_api.Services.Authentication;

public interface IAuthStrategyFactory
{
    IAuthStrategy GetStrategy(string provider);
}
