using System.Collections.Generic;
using System.Text.Json;

namespace jwt_rest_api.Models.Dto;

public class GameProgressDto
{
    public int Level { get; set; }
    public int Score { get; set; }
    public int Coins { get; set; }
    public List<string> Inventory { get; set; } = new();
    
    // Custom JSON payload (e.g. coordinates, inventory stats)
    public JsonElement StateData { get; set; }
}
