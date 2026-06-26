using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using jwt_rest_api.Services;

namespace jwt_rest_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Added to protect the dashboard data
public class AdminController : BaseApiController
{
    private readonly IGameService _gameService;

    public AdminController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _gameService.GetAdminDashboardDataAsync();
        if (!result.IsSuccess)
        {
            return HandleResult(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("seed-dummy")]
    [AllowAnonymous]
    public async Task<IActionResult> SeedDummyData([FromServices] jwt_rest_api.Data.GameDbContext dbContext)
    {
        // 1. Pastikan minimal ada 1 player di database
        var user = dbContext.Users.FirstOrDefault();
        if (user == null)
        {
            return BadRequest(new { error = "Belum ada player di database. Harap login minimal 1 kali lewat Unity/Google agar User terdaftar." });
        }

        if (!dbContext.EventDataMaster.Any(e => e.EventName == "First Day"))
        {
            var eventMaster = new Models.EventData { EventName = "First Day", ArcName = "Tutorial" };
            dbContext.EventDataMaster.Add(eventMaster);
            await dbContext.SaveChangesAsync(); // Save to generate ID
            
            var subEventMaster = new Models.SubEventData { EncounterDomain = "O1", EncounterValue = false, EventId = eventMaster.Id };
            dbContext.SubEventDataMaster.Add(subEventMaster);
            await dbContext.SaveChangesAsync();
        }

        var master = dbContext.EventDataMaster.First();

        // 3. Buat 1 Dummy Session lengkap dengan hierarkinya
        var session = new Models.GameSession
        {
            Id = Guid.NewGuid().ToString(),
            UserId = user.Id,
            SessionDurationSeconds = 1200,
            SessionStartDateTime = System.DateTime.UtcNow.AddMinutes(-20),
            SessionEndDateTime = System.DateTime.UtcNow,
            SessionIsFinished = true
        };

        var evt = new Models.SessionEvent
        {
            Id = Guid.NewGuid().ToString(),
            Session = session,
            EventDataId = master.Id,
            EventDurationSeconds = 300,
            EventStartDateTime = System.DateTime.UtcNow.AddMinutes(-15),
            EventEndDateTime = System.DateTime.UtcNow.AddMinutes(-10)
        };

        var encounter = new Models.EventEncounter
        {
            Id = Guid.NewGuid().ToString(),
            SessionEvent = evt,
            EncounterObject = "Torvak",
            EncounterQuestion = "I need more iron, let's go to mountain!",
            EncounterDurationSeconds = 60,
            EncounterStartDateTime = System.DateTime.UtcNow.AddMinutes(-15),
            EncounterEndDateTime = System.DateTime.UtcNow.AddMinutes(-14)
        };

        var response = new Models.PlayerResponse
        {
            Encounter = encounter,
            ResponseText = "ok, let's do it, i wanna know how to mining",
            Domain = "O1",
            Weight = 4
        };

        var minigame = new Models.SessionMinigame
        {
            Id = Guid.NewGuid().ToString(),
            SessionEvent = evt,
            MinigameDurationSeconds = 240,
            MinigameStartDateTime = System.DateTime.UtcNow.AddMinutes(-14),
            MinigameEndDateTime = System.DateTime.UtcNow.AddMinutes(-10),
            MinigameIsRelatedWithEncounterId = encounter.Id
        };

        dbContext.GameSessions.Add(session);
        dbContext.SessionEvents.Add(evt);
        dbContext.EventEncounters.Add(encounter);
        dbContext.PlayerResponses.Add(response);
        dbContext.SessionMinigames.Add(minigame);

        await dbContext.SaveChangesAsync();

        return Ok(new { message = "Data dummy (Session, Event, Encounter, Response, Minigame) berhasil ditambahkan ke Player: " + user.Name });
    }

    [HttpGet("seed-ipip")]
    [AllowAnonymous]
    public async Task<IActionResult> SeedIpipData([FromServices] jwt_rest_api.Data.GameDbContext dbContext)
    {
        // 1. Buat/Cek Event Master untuk IPIP-NEO-60
        var eventMaster = dbContext.EventDataMaster.FirstOrDefault(e => e.EventName == "IPIP-NEO-60 Assessment");
        if (eventMaster == null)
        {
            eventMaster = new Models.EventData { EventName = "IPIP-NEO-60 Assessment", ArcName = "Psychological Test" };
            dbContext.EventDataMaster.Add(eventMaster);
            await dbContext.SaveChangesAsync();
        }

        int addedCount = 0;
        
        // 2. Loop melalui kamus IpipNeo60Dictionary dan masukkan ke SubEventDataMaster (sebaga referensi soal)
        foreach (var q in jwt_rest_api.Common.IpipNeo60Dictionary.Questions)
        {
            // Cek apakah soal ini sudah ada
            var existing = dbContext.SubEventDataMaster.FirstOrDefault(s => s.EventId == eventMaster.Id && s.EncounterDomain == q.Facet && s.EncounterValue == q.IsReverse);
            if (existing == null)
            {
                // Kita gunakan EncounterDomain untuk menyimpan Facet (N1, E2, dst)
                // EncounterValue = true berarti reverse-scored, false berarti normal-scored
                var subEvent = new Models.SubEventData
                {
                    EventId = eventMaster.Id,
                    EncounterDomain = q.Facet, 
                    EncounterValue = q.IsReverse
                };
                dbContext.SubEventDataMaster.Add(subEvent);
                addedCount++;
            }
        }

        await dbContext.SaveChangesAsync();

        return Ok(new { message = $"Berhasil melakukan seeding IPIP-NEO-60. {addedCount} soal baru ditambahkan ke Master Data." });
    }
}
