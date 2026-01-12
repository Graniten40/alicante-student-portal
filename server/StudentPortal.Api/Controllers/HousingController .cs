using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Data;
using StudentPortal.Api.Domain.Housing;
using StudentPortal.Api.Domain;
using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/students/{studentId}/housing")]
[Authorize]
public class HousingController : ControllerBase
{
    private readonly AppDbContext _db;

    public HousingController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<HousingDto>> Get(string studentId)
    {
        var housing = await _db.StudentHousing
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.StudentId == studentId);

        if (housing is null)
            return Ok(new HousingDto(HousingStatus.Unknown, null, null, null, false, null));

        return Ok(new HousingDto(
            housing.Status, housing.AddressLine, housing.City,
            housing.MoveInDate, housing.ContractUploaded, housing.Notes
        ));
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HousingDto>> Upsert(string studentId, UpsertHousingDto dto)
    {
        var housing = await _db.StudentHousing.FirstOrDefaultAsync(x => x.StudentId == studentId);

        if (housing is null)
        {
            housing = new StudentHousing { StudentId = studentId };
            _db.StudentHousing.Add(housing);
        }

        housing.Status = dto.Status;
        housing.AddressLine = dto.AddressLine;
        housing.City = dto.City;
        housing.MoveInDate = dto.MoveInDate;
        housing.ContractUploaded = dto.ContractUploaded;
        housing.Notes = dto.Notes;
        housing.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new HousingDto(
            housing.Status, housing.AddressLine, housing.City,
            housing.MoveInDate, housing.ContractUploaded, housing.Notes
        ));
    }
}
