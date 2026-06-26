using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace jwt_rest_api.Models.Dto;

public class AdminDashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalRequests { get; set; }
    public List<int> TrafficData { get; set; } = new();
    public List<PlayerStatDto> Players { get; set; } = new();
}

public class PlayerStatDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Score { get; set; }
    public int Coins { get; set; }
    
    // Extracted from StateDataJson
    public int DayReached { get; set; }
    public int PhaseReached { get; set; }
    
    // Extracted from InventoryJson or StateDataJson
    public int TotalInteractions { get; set; }
    
    // Simple assessment derived from data
    public string PsychoProfile { get; set; } = "Neutral";
    
    public DateTime LastUpdated { get; set; }
}
