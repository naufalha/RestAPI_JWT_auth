using System.ComponentModel.DataAnnotations;

namespace jwt_rest_api.Models.Dto;

public class TestAuthRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;
}
