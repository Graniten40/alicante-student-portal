using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Data;
using StudentPortal.Api.Domain;
using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/students/{studentId}/contacts")]
[Authorize(Roles = "Admin")]
public class ContactLogController : ControllerBase
{
    private readonly AppDbContext _db;
    public ContactLogController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactLogItemDto>>> List(string studentId)
    {
        var items = await _db.StudentContactLogs
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.ContactedAtUtc)
            .Select(x => new ContactLogItemDto(
                x.Id, x.Type, x.Channel, x.ContactName, x.Organization, x.Reference,
                x.Summary, x.Details, x.ContactedAtUtc
            ))
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ContactLogItemDto>> Create(string studentId, CreateContactLogItemDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Missing user id");

        var entity = new StudentContactLog
        {
            StudentId = studentId,
            Type = dto.Type,
            Channel = dto.Channel,
            ContactName = dto.ContactName,
            Organization = dto.Organization,
            Reference = dto.Reference,
            Summary = dto.Summary,
            Details = dto.Details,
            ContactedAtUtc = dto.ContactedAtUtc ?? DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _db.StudentContactLogs.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(List), new { studentId }, new ContactLogItemDto(
            entity.Id, entity.Type, entity.Channel, entity.ContactName, entity.Organization, entity.Reference,
            entity.Summary, entity.Details, entity.ContactedAtUtc
        ));
    }
}
