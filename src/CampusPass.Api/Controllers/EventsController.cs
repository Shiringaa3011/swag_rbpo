using CampusPass.Api.Contracts;
using CampusPass.Api.Data;
using CampusPass.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusPass.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly CampusPassDbContext _db;

    public EventsController(CampusPassDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventResponse>>> GetPublished(
        CancellationToken cancellationToken)
    {
        var events = await _db.Events
            .AsNoTracking()
            .Where(e => e.State == EventState.Published)
            .OrderBy(e => e.StartsAt)
            .Select(e => new EventResponse(
                e.Id,
                e.Title,
                e.Description,
                e.StartsAt,
                e.Capacity,
                e.Capacity - e.RegisteredCount))
            .ToListAsync(cancellationToken);

        return Ok(events);
    }
}
