namespace StudentPortal.Api.Dtos;

public sealed record MyPackageDto(
    Guid ProductId,
    string Slug,
    string Title,
    DateTimeOffset? ExpiresAt,
    bool IsActive
);
