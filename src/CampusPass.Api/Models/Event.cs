namespace CampusPass.Api.Models;

public enum EventState
{
    Draft,
    Published
}

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartsAt { get; set; }
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public Guid AuthorId { get; set; }
    public EventState State { get; set; }

    public static Event Create(
        string title,
        string description,
        DateTime startsAt,
        int capacity,
        Guid authorId,
        DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Event title is required.", nameof(title));
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Event capacity must be positive.");
        if (startsAt <= utcNow)
            throw new ArgumentOutOfRangeException(
                nameof(startsAt),
                "Event start time must be in the future.");

        return new Event
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description.Trim(),
            StartsAt = startsAt,
            Capacity = capacity,
            RegisteredCount = 0,
            AuthorId = authorId,
            State = EventState.Draft
        };
    }
}
