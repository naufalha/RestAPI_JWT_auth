namespace jwt_rest_api.Models;

public class SubEventData
{
    public int EncounterId { get; set; }
    public int EventId { get; set; }
    public string EncounterDomain { get; set; } = null!;
    public bool EncounterValue { get; set; }

    public EventData EventData { get; set; } = null!;
}
