using StudentPortal.Api.Domain;

namespace StudentPortal.Api.Dtos;

public record TimelineEventDto(
    int Id,
    TimelineCategory Category,
    TimelineStatus Status,
    string Title,
    string? Description,
    DateTime EventAtUtc
);