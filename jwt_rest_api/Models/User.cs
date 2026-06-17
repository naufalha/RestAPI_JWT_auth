using System;

namespace jwt_rest_api.Models;

public class User
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string GoogleSubjectId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
