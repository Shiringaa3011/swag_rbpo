namespace CampusPass.Api.Contracts;

public sealed record EventResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime StartsAt,
    int Capacity,
    int FreePlaces);
