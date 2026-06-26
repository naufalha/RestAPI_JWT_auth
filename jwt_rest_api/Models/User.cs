using System;
using System.Collections.Generic;

namespace jwt_rest_api.Models;

public class User
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string GoogleSubjectId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public int SessionCount { get; set; }
    public string? LastPlayedSessionId { get; set; }

    public ICollection<NpcInteraction> NpcInteractions { get; set; } = new List<NpcInteraction>();
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}
