namespace StudentPortal.Api.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Slug { get; set; } = default!;
    public string TitleEn { get; set; } = default!;
    public string TitleZh { get; set; } = default!;
    public string DescriptionEn { get; set; } = default!;
    public string DescriptionZh { get; set; } = default!;
    public int PriceCents { get; set; }
    public bool IsFree { get; set; }
    public bool IsActive { get; set; } = true;
}
