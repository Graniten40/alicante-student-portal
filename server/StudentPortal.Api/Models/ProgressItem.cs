namespace StudentPortal.Api.Models;

public class ProgressItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = default!;
    public Guid ProductId { get; set; }
    public string StepKey { get; set; } = default!;
    public string TaskKey { get; set; } = default!;
    public bool IsDone { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
