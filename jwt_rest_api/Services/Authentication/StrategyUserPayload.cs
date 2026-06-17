namespace jwt_rest_api.Services.Authentication;

public class StrategyUserPayload
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
}
