using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Data;
using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Services;

public sealed class PortalService : IPortalService
{
    private readonly AppDbContext _db;

    public PortalService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MyPackageDto>> GetMyPackagesAsync(
        string userId,
        string lang,
        DateTimeOffset now,
        CancellationToken ct)
    {
        var isZh = string.Equals(lang?.Trim(), "zh", StringComparison.OrdinalIgnoreCase);

        var rows = await _db.Entitlements
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .Join(_db.Products.AsNoTracking(),
                e => e.ProductId,
                p => p.Id,
                (e, p) => new
                {
                    p.Id,
                    p.Slug,
                    Title = isZh ? p.TitleZh : p.TitleEn,
                    e.ValidUntil,
                    IsActive = (e.ValidUntil == null) || (e.ValidUntil > now)
                })
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Title)
            .ToListAsync(ct);

        return rows
            .Select(x => new MyPackageDto(
                x.Id,
                x.Slug,
                x.Title,
                x.ValidUntil,
                x.IsActive
            ))
            .ToList();
    }

    public async Task<PortalAccessResult> GetPackageIfEntitledAsync(
        string userId,
        string packageSlug,
        string lang,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var isZh = string.Equals(lang?.Trim(), "zh", StringComparison.OrdinalIgnoreCase);

        var pkg = await _db.Products
            .AsNoTracking()
            .Where(p => p.Slug == packageSlug)
            .Select(p => new
            {
                p.Id,
                p.Slug,
                Title = isZh ? p.TitleZh : p.TitleEn,
                Description = isZh ? p.DescriptionZh : p.DescriptionEn
            })
            .SingleOrDefaultAsync(ct);

        if (pkg is null)
            return new PortalAccessResult(PortalAccessStatus.NotFound, null);

        var entitled = await _db.Entitlements
            .AsNoTracking()
            .AnyAsync(e =>
                e.UserId == userId &&
                e.ProductId == pkg.Id &&
                (e.ValidUntil == null || e.ValidUntil > now),
                ct);

        if (!entitled)
            return new PortalAccessResult(PortalAccessStatus.Forbidden, null);

        // Senaste publicerade content
        var content = await _db.PackContents
            .AsNoTracking()
            .Where(pc => pc.ProductId == pkg.Id)
            .OrderByDescending(pc => pc.PublishedAt)
            .Select(pc => new PortalPackageContentDto(
                pc.Version,
                pc.JsonContent,
                pc.PublishedAt
            ))
            .FirstOrDefaultAsync(ct);

        var dto = new PortalPackageDto(
            pkg.Id,
            pkg.Slug,
            pkg.Title,
            pkg.Description,
            content
        );

        return new PortalAccessResult(PortalAccessStatus.Ok, dto);
    }

    public async Task<List<MyPackageDto>> GetMyPackagesByMarketAsync(
        string userId,
        string marketCode,
        string lang,
        DateTimeOffset now,
        CancellationToken ct)
    {
        var isZh = string.Equals(lang?.Trim(), "zh", StringComparison.OrdinalIgnoreCase);

        var marketId = await _db.Markets
            .AsNoTracking()
            .Where(m => m.Code == marketCode && m.IsActive)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(ct);

        if (marketId is null)
            return new List<MyPackageDto>();

        var rows = await _db.Entitlements
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .Join(_db.Products.AsNoTracking(),
                e => e.ProductId,
                p => p.Id,
                (e, p) => new
                {
                    p.Id,
                    p.Slug,
                    p.MarketId,
                    Title = isZh ? p.TitleZh : p.TitleEn,
                    e.ValidUntil,
                    IsActive = (e.ValidUntil == null) || (e.ValidUntil > now)
                })
            .Where(x => x.MarketId == marketId.Value)
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Title)
            .ToListAsync(ct);

        return rows
            .Select(x => new MyPackageDto(
                x.Id,
                x.Slug,
                x.Title,
                x.ValidUntil,
                x.IsActive
            ))
            .ToList();
    }


    public async Task<PortalAccessResult> GetPackageIfEntitledByMarketAsync(
        string userId,
        string marketCode,
        string packageSlug,
        string lang,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var isZh = string.Equals(lang?.Trim(), "zh", StringComparison.OrdinalIgnoreCase);

        // Hämta marketId först (stabilare än att gå via navigation p.Market.Code)
        var marketId = await _db.Markets
            .AsNoTracking()
            .Where(m => m.Code == marketCode && m.IsActive)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(ct);

        if (marketId is null)
            return new PortalAccessResult(PortalAccessStatus.NotFound, null);

        var pkg = await _db.Products
            .AsNoTracking()
            .Where(p => p.MarketId == marketId.Value && p.Slug == packageSlug)
            .Select(p => new
            {
                p.Id,
                p.Slug,
                Title = isZh ? p.TitleZh : p.TitleEn,
                Description = isZh ? p.DescriptionZh : p.DescriptionEn
            })
            .SingleOrDefaultAsync(ct);

        if (pkg is null)
            return new PortalAccessResult(PortalAccessStatus.NotFound, null);

        var entitled = await _db.Entitlements
            .AsNoTracking()
            .AnyAsync(e =>
                e.UserId == userId &&
                e.ProductId == pkg.Id &&
                (e.ValidUntil == null || e.ValidUntil > now),
                ct);

        if (!entitled)
            return new PortalAccessResult(PortalAccessStatus.Forbidden, null);

        var content = await _db.PackContents
            .AsNoTracking()
            .Where(pc => pc.ProductId == pkg.Id)
            .OrderByDescending(pc => pc.PublishedAt)
            .Select(pc => new PortalPackageContentDto(
                pc.Version,
                pc.JsonContent,
                pc.PublishedAt
            ))
            .FirstOrDefaultAsync(ct);

        var dto = new PortalPackageDto(
            pkg.Id,
            pkg.Slug,
            pkg.Title,
            pkg.Description,
            content
        );

        return new PortalAccessResult(PortalAccessStatus.Ok, dto);
    }

}
