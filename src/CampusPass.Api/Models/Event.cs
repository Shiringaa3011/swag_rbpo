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
}
