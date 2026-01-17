using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Data;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/markets")]
public class MarketsController : ControllerBase
{
    private readonly AppDbContext _db;
    public MarketsController(AppDbContext db) => _db = db;

    // GET /api/markets
    [HttpGet]
    public async Task<IActionResult> GetMarkets()
    {
        var markets = await _db.Markets
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .Select(m => new
            {
                m.Code,
                m.Name,
                m.Currency,
                m.TimeZone
            })
            .ToListAsync();

        return Ok(markets);
    }

    // GET /api/markets/{code}/products
    [HttpGet("{code}/products")]
    public async Task<IActionResult> GetProducts([FromRoute] string code)
    {
        var marketId = await _db.Markets
            .Where(m => m.Code == code && m.IsActive)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync();

        if (marketId is null)
            return NotFound(new { message = $"Market '{code}' not found." });

        var products = await _db.Products
            .Where(p => p.MarketId == marketId.Value && p.IsActive)
            .OrderBy(p => p.PriceCents)
            .Select(p => new
            {
                p.Id,
                p.Slug,
                p.TitleEn,
                p.TitleZh,
                p.DescriptionEn,
                p.DescriptionZh,
                p.PriceCents,
                p.IsFree
            })
            .ToListAsync();

        return Ok(products);
    }

    // GET /api/markets/{code}/products/{slug}/content
    [HttpGet("{code}/products/{slug}/content")]
    public async Task<IActionResult> GetLatestContent([FromRoute] string code, [FromRoute] string slug)
    {
        var marketId = await _db.Markets
            .Where(m => m.Code == code && m.IsActive)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync();

        if (marketId is null)
            return NotFound(new { message = $"Market '{code}' not found." });

        var product = await _db.Products
            .Where(p => p.MarketId == marketId.Value && p.Slug == slug && p.IsActive)
            .Select(p => new { p.Id, p.Slug })
            .FirstOrDefaultAsync();

        if (product is null)
            return NotFound(new { message = $"Product '{slug}' not found in market '{code}'." });

        var content = await _db.PackContents
            .Where(c => c.ProductId == product.Id)
            .OrderByDescending(c => c.Version)
            .Select(c => new
            {
                c.Version,
                c.PublishedAt,
                c.JsonContent
            })
            .FirstOrDefaultAsync();

        if (content is null)
            return NotFound(new { message = "No content published for this product yet." });

        return Ok(content);
    }
}
