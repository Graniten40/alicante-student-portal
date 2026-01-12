namespace StudentPortal.Api.Dtos;

public sealed record PortalPackageDto(
    Guid ProductId,
    string Slug,
    string Title,
    string? Description,
    PortalPackageContentDto? Content
);