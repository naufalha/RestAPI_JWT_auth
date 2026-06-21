using System.ComponentModel.DataAnnotations;

namespace jwt_rest_api.Models.Dto;

public class EmailAuthRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
