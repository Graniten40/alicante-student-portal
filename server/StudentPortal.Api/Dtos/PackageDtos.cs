namespace StudentPortal.Api.Dtos;

public record PackageCardDto(string Slug, string TitleEn, string TitleZh, string DescriptionEn, string DescriptionZh, int PriceCents, bool IsFree);
