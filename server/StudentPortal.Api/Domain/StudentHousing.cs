using StudentPortal.Api.Auth;

namespace StudentPortal.Api.Domain.Housing;

public class StudentHousing
{
    public int Id { get; set; }

    // Koppling till Identity-user
    public string StudentId { get; set; } = default!;
    public AppUser Student { get; set; } = default!;

    public HousingStatus Status { get; set; } = HousingStatus.Searching;

    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public DateTime? MoveInDate { get; set; }

    public bool ContractUploaded { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
