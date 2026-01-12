namespace StudentPortal.Api.Dtos;

public sealed record PortalContentItemDto(
  Guid ContentProductId,
  string Slug,
  string Title,
  string? Kind,          // t.ex. "video", "pdf", "article" om du har
  string? UrlOrPath      // om du har en länk/filväg
);