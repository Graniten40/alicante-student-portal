using StudentPortal.Api.Auth;

namespace StudentPortal.Api.Domain;

public class StudentContactLog
{
    public int Id { get; set; }

    public string StudentId { get; set; } = default!;
    public AppUser Student { get; set; } = default!;

    public ContactType Type { get; set; }
    public ContactChannel Channel { get; set; }

    public string? ContactName { get; set; }      // landlord name / school contact
    public string? Organization { get; set; }     // school name / agency
    public string? Reference { get; set; }        // email/phone/case id

    public string Summary { get; set; } = default!;
    public string? Details { get; set; }

    public DateTime ContactedAtUtc { get; set; } = DateTime.UtcNow;

    public string CreatedByUserId { get; set; } = default!;
    public AppUser CreatedByUser { get; set; } = default!;
}