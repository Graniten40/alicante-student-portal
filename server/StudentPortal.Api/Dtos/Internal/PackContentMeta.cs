namespace StudentPortal.Api.Dtos;
public sealed record PackContentMeta(
    string? Slug,
    string? Title,
    string? Kind,
    string? UrlOrPath
);
