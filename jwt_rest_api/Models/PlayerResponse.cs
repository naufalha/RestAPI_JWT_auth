namespace jwt_rest_api.Models;

public class PlayerResponse
{
    public int Id { get; set; }
    public string EncounterId { get; set; } = null!; // FK to EventEncounter
    public string ResponseText { get; set; } = null!;
    public string Domain { get; set; } = null!; // e.g. "Openness"
    public int Weight { get; set; } // +1 or -1

    public EventEncounter Encounter { get; set; } = null!;
}
