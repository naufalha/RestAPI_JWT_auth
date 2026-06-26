using System;
using System.Collections.Generic;

namespace jwt_rest_api.Models;

public class GameSession
{
    public string Id { get; set; } = null!; // SessionId
    public string UserId { get; set; } = null!;
    public int SessionDurationSeconds { get; set; }
    public DateTime SessionStartDateTime { get; set; }
    public DateTime SessionEndDateTime { get; set; }
    public bool SessionIsFinished { get; set; }

    public User User { get; set; } = null!;
    public ICollection<SessionEvent> Events { get; set; } = new List<SessionEvent>();
}
