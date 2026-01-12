using StudentPortal.Api.Domain;

namespace StudentPortal.Api.Dtos;

public record ContactLogItemDto(
    int Id,
    ContactType Type,
    ContactChannel Channel,
    string? ContactName,
    string? Organization,
    string? Reference,
    string Summary,
    string? Details,
    DateTime ContactedAtUtc
);