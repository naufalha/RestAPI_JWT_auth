using System;
using System.Collections.Generic;

namespace jwt_rest_api.Models;

public class SessionEvent
{
    public string Id { get; set; } = null!; // eventId (instance ID)
    public string SessionId { get; set; } = null!;
    public int EventDataId { get; set; } // Reference to EventData master table
    public int EventDurationSeconds { get; set; }
    public DateTime EventStartDateTime { get; set; }
    public DateTime EventEndDateTime { get; set; }

    public GameSession Session { get; set; } = null!;
    public EventData EventData { get; set; } = null!;

    public ICollection<EventEncounter> Encounters { get; set; } = new List<EventEncounter>();
    public ICollection<SessionMinigame> Minigames { get; set; } = new List<SessionMinigame>();
}
