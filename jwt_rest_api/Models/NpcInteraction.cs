using System;
using System.Text.Json.Serialization;

namespace jwt_rest_api.Models;

public class NpcInteraction
{
    public int Id { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    
    public string NpcId { get; set; } = string.Empty;
    
    public string InteractionType { get; set; } = string.Empty;
    
    public string PsychoImpact { get; set; } = "Neutral";
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public User? User { get; set; }
}
