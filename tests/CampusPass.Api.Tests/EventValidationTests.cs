using CampusPass.Api.Models;
using CampusPass.Api.Contracts;
using CampusPass.Api.Controllers;
using CampusPass.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CampusPass.Api.Tests;

public sealed class EventValidationTests
{
    private static readonly DateTime UtcNow =
        new(2026, 10, 12, 12, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_RejectsNonPositiveCapacity(int capacity)
    {
        var action = () => CreateEvent(UtcNow.AddDays(1), capacity);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Create_RejectsStartTimeThatIsNotInFuture()
    {
        var action = () => CreateEvent(UtcNow, 10);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Create_AcceptsFutureEventWithPositiveCapacity()
    {
        var startsAt = UtcNow.AddDays(1);

        var created = CreateEvent(startsAt, 25);

        Assert.Equal("Security workshop", created.Title);
        Assert.Equal(startsAt, created.StartsAt);
        Assert.Equal(25, created.Capacity);
        Assert.Equal(0, created.RegisteredCount);
        Assert.Equal(EventState.Draft, created.State);
    }

    [Fact]
    public async Task GetPublished_DoesNotReturnDraftEvents()
    {
        var options = new DbContextOptionsBuilder<CampusPassDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new CampusPassDbContext(options);
        var authorId = Guid.NewGuid();
        var published = Event.Create(
            "Published workshop",
            "Visible event",
            UtcNow.AddDays(1),
            20,
            authorId,
            UtcNow);
        published.State = EventState.Published;
        var draft = Event.Create(
            "Draft workshop",
            "Hidden event",
            UtcNow.AddDays(2),
            20,
            authorId,
            UtcNow);
        db.Events.AddRange(published, draft);
        await db.SaveChangesAsync();
        var controller = new EventsController(db);

        var result = await controller.GetPublished(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var events = Assert.IsAssignableFrom<IReadOnlyList<EventResponse>>(ok.Value);
        var item = Assert.Single(events);
        Assert.Equal(published.Id, item.Id);
        Assert.DoesNotContain(events, value => value.Id == draft.Id);
    }

    private static Event CreateEvent(DateTime startsAt, int capacity) =>
        Event.Create(
            " Security workshop ",
            "S06 validation",
            startsAt,
            capacity,
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UtcNow);
}
