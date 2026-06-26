using System;

namespace jwt_rest_api.Models;

public class SessionMinigame
{
    public string Id { get; set; } = null!; // minigameId
    public string EventId { get; set; } = null!; // SessionEvent Id
    public int MinigameDurationSeconds { get; set; }
    public DateTime MinigameStartDateTime { get; set; }
    public DateTime MinigameEndDateTime { get; set; }
    public string? MinigameIsRelatedWithEncounterId { get; set; } // Nullable encounter ID

    public SessionEvent SessionEvent { get; set; } = null!;
}
