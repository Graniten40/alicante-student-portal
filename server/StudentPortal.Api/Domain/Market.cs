namespace StudentPortal.Api.Domain;

public class Market
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;   // "TH-CNX"
    public string Name { get; set; } = default!;   // "Chiang Mai"
    public string Currency { get; set; } = "THB";  // valfritt
    public string TimeZone { get; set; } = "Asia/Bangkok";
    public bool IsActive { get; set; } = true;
}
