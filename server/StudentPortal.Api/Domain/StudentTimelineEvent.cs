using StudentPortal.Api.Auth;

namespace StudentPortal.Api.Domain;

public class StudentTimelineEvent
{
    public int Id { get; set; }

    public string StudentId { get; set; } = default!;
    public AppUser Student { get; set; } = default!;

    public TimelineCategory Category { get; set; }
    public TimelineStatus Status { get; set; } = TimelineStatus.Info;

    public string Title { get; set; } = default!;
    public string? Description { get; set; }

    public DateTime EventAtUtc { get; set; } = DateTime.UtcNow;

    public string CreatedByUserId { get; set; } = default!;
    public AppUser CreatedByUser { get; set; } = default!;
}