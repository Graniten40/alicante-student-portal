namespace StudentPortal.Api.Models;

public class Entitlement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = default!;
    public Guid ProductId { get; set; }
    public string Source { get; set; } = "Free"; // Free/Stripe
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ValidUntil { get; set; }
    public DateTime GrantedAtUtc { get; set; } = DateTime.UtcNow;
    
    // navigation
    public Product? Product { get; set; } = default!;
}
