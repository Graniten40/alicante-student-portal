using StudentPortal.Api.Domain;

namespace StudentPortal.Api.Dtos;

public record CreateTimelineEventDto(
    TimelineCategory Category,
    TimelineStatus Status,
    string Title,
    string? Description,
    DateTime? EventAtUtc
);