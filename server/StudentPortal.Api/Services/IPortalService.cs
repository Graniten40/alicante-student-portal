using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Services;

public interface IPortalService
{
    Task<List<MyPackageDto>> GetMyPackagesAsync(string userId, string lang, CancellationToken ct);

    Task<PortalAccessResult> GetPackageIfEntitledAsync(string userId, string packageSlug, string lang, CancellationToken ct);
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
