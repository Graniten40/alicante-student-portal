using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Data;
using StudentPortal.Api.Domain;
using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/students/{studentId}/timeline")]
[Authorize]
public class TimelineController : ControllerBase
{
    private readonly AppDbContext _db;
    public TimelineController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimelineEventDto>>> List(string studentId)
    {
        // Enkel variant: Admin ser alla; student ska bara se sina egna
        // (lägg hårdare policy senare om du vill)
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != studentId) return Forbid();
        }

        var items = await _db.StudentTimelineEvents
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.EventAtUtc)
            .Select(x => new TimelineEventDto(x.Id, x.Category, x.Status, x.Title, x.Description, x.EventAtUtc))
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TimelineEventDto>> Create(string studentId, CreateTimelineEventDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Missing user id");

        var entity = new StudentTimelineEvent
        {
            StudentId = studentId,
            Category = dto.Category,
            Status = dto.Status,
            Title = dto.Title,
            Description = dto.Description,
            EventAtUtc = dto.EventAtUtc ?? DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _db.StudentTimelineEvents.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(List), new { studentId }, new TimelineEventDto(
            entity.Id, entity.Category, entity.Status, entity.Title, entity.Description, entity.EventAtUtc
        ));
    }
}
