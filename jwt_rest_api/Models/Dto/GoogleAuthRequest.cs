using System.ComponentModel.DataAnnotations;

namespace jwt_rest_api.Models.Dto;

public class GoogleAuthRequest
{
    [Required]
    public string IdToken { get; set; } = null!;
}
