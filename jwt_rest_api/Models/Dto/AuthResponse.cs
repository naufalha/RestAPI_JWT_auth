namespace jwt_rest_api.Models.Dto;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
}
