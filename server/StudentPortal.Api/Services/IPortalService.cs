using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Services;

public interface IPortalService
{
    // Legacy (utan market)
    Task<List<MyPackageDto>> GetMyPackagesAsync(string userId, string lang, DateTimeOffset now, CancellationToken ct);
    Task<PortalAccessResult> GetPackageIfEntitledAsync(string userId, string packageSlug, string lang, CancellationToken ct);

    // Market-aware (ny)
    Task<List<MyPackageDto>> GetMyPackagesByMarketAsync(string userId, string marketCode, string lang, DateTimeOffset now, CancellationToken ct);
    Task<PortalAccessResult> GetPackageIfEntitledByMarketAsync(string userId, string marketCode, string packageSlug, string lang, CancellationToken ct);
}

public enum PortalAccessStatus
{
    Ok = 0,
    Forbidden = 1,
    NotFound = 2
}

public sealed record PortalAccessResult(
    PortalAccessStatus Status,
    PortalPackageDto? Package
);
