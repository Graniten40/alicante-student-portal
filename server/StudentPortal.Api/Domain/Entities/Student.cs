using StudentPortal.Api.Domain;

namespace StudentPortal.Api.Entities;

public class Student
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }

    public SupportTier SupportTier { get; set; } = SupportTier.Free;

    // Lägre = högre prio (0 = normal)
    public int SupportPriority { get; set; } = 0;
}
