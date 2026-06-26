using System;
using System.Collections.Generic;

namespace jwt_rest_api.Models;

public class EventEncounter
{
    public string Id { get; set; } = null!; // encounterId
    public string EventId { get; set; } = null!; // SessionEvent Id
    public string EncounterObject { get; set; } = null!; // e.g. "Torvak"
    public string EncounterQuestion { get; set; } = null!;
    public int EncounterDurationSeconds { get; set; }
    public DateTime EncounterStartDateTime { get; set; }
    public DateTime EncounterEndDateTime { get; set; }

    public SessionEvent SessionEvent { get; set; } = null!;
    public ICollection<PlayerResponse> PlayerResponses { get; set; } = new List<PlayerResponse>();
}
