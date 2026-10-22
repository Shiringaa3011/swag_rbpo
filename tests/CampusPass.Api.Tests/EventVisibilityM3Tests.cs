
using CampusPass.Api.Contracts;
using CampusPass.Api.Controllers;
using CampusPass.Api.Data;
using CampusPass.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CampusPass.Api.Tests;

public sealed class EventVisibilityM3Tests
{
    private static readonly DateTime Now =
        new(2026, 10, 12, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task GetPublished_WhenAllEventsAreDrafts_ReturnsEmptyList()
    {
        await using var db = CreateDatabase();

        db.Events.Add(CreateEvent("Draft A", 1, EventState.Draft));
        db.Events.Add(CreateEvent("Draft B", 2, EventState.Draft));
        await db.SaveChangesAsync();

        var controller = new EventsController(db);

        var result = await controller.GetPublished(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var events = Assert.IsAssignableFrom<IReadOnlyList<EventResponse>>(ok.Value);

        Assert.Empty(events);
    }

    [Fact]
    public async Task GetPublished_ReturnsOnlyPublishedEventsInStartOrder()
    {
        await using var db = CreateDatabase();

        var later = CreateEvent("Published later", 3, EventState.Published);
        var hidden = CreateEvent("Secret draft", 2, EventState.Draft);
        var earlier = CreateEvent("Published earlier", 1, EventState.Published);

        db.Events.AddRange(later, hidden, earlier);
        await db.SaveChangesAsync();

        var controller = new EventsController(db);

        var result = await controller.GetPublished(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var events = Assert.IsAssignableFrom<IReadOnlyList<EventResponse>>(ok.Value);

        Assert.Equal(2, events.Count);
        Assert.Equal(earlier.Id, events[0].Id);
        Assert.Equal(later.Id, events[1].Id);
        Assert.DoesNotContain(events, e => e.Id == hidden.Id);
    }

    private static CampusPassDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<CampusPassDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CampusPassDbContext(options);
    }

    private static Event CreateEvent(
        string title,
        int daysFromNow,
        EventState state)
    {
        var item = Event.Create(
            title,
            "S06 M3 security test",
            Now.AddDays(daysFromNow),
            20,
            Guid.NewGuid(),
            Now);

        item.State = state;
        return item;
    }
}
