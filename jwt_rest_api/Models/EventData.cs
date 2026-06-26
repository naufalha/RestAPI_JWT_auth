using System.Collections.Generic;

namespace jwt_rest_api.Models;

public class EventData
{
    public int Id { get; set; }
    public string EventName { get; set; } = null!;
    public string ArcName { get; set; } = null!;

    public ICollection<SubEventData> SubEvents { get; set; } = new List<SubEventData>();
}
