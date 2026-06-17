using System;
using System.Collections.Generic;

namespace jwt_rest_api.Models;

public class GameProgress
{
    public string UserId { get; set; } = null!;
    public int Level { get; set; }
    public int Score { get; set; }
    public int Coins { get; set; }
    
    // Stored as JSON string in the database
    public string InventoryJson { get; set; } = "[]";
    
    // Stored as JSON string in the database
    public string StateDataJson { get; set; } = "{}";
    
    public DateTime LastUpdated { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
