namespace StudentPortal.Api.Models;

public class PackContent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public int Version { get; set; } = 1;
    public string JsonContent { get; set; } = default!;
    public DateTimeOffset PublishedAt { get; set; } = DateTimeOffset.UtcNow;

    public Product? Product { get; set; }
}
