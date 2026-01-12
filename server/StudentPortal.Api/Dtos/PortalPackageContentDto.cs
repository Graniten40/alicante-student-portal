namespace StudentPortal.Api.Dtos;

public sealed record PortalPackageContentDto(
    int Version,
    string JsonContent,
    DateTimeOffset PublishedAt
);