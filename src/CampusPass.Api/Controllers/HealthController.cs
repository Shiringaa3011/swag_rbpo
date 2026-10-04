using CampusPass.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusPass.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly CampusPassDbContext _db;

    public HealthController(CampusPassDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var databaseAvailable =
            await _db.Database.CanConnectAsync(cancellationToken);

        return databaseAvailable
            ? Ok(new { status = "ok", database = "ok" })
            : StatusCode(503, new
            {
                status = "degraded",
                database = "unavailable"
            });
    }
}
