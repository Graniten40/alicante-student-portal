using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Data;
using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/packages")]
public class PackagesController : ControllerBase
{
    private readonly AppDbContext _db;
    public PackagesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IEnumerable<PackageCardDto>> GetAll()
    {
        return await _db.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.PriceCents)
            .Select(p => new PackageCardDto(p.Slug, p.TitleEn, p.TitleZh, p.DescriptionEn, p.DescriptionZh, p.PriceCents, p.IsFree))
            .ToListAsync();
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<PackageCardDto>> GetOne(string slug)
    {
        var p = await _db.Products.FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);
        if (p == null) return NotFound();

        return Ok(new PackageCardDto(p.Slug, p.TitleEn, p.TitleZh, p.DescriptionEn, p.DescriptionZh, p.PriceCents, p.IsFree));
    }
}
