using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Api.Dtos;
using StudentPortal.Api.Services;
using System.Security.Claims;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/portal")]
[Produces("application/json")]
[Authorize]
public sealed class PortalController : ControllerBase
{
    private readonly IPortalService _portal;

    public PortalController(IPortalService portal) => _portal = portal;

    // -------------------------
    // Legacy (utan market)
    // -------------------------

    [HttpGet("my-packages")]
    [ProducesResponseType(typeof(IReadOnlyList<MyPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<MyPackageDto>>> MyPackages(CancellationToken ct)
    {
        if (!TryGetUser(out var userId, out var lang))
            return Unauthorized();

        var now = DateTimeOffset.UtcNow;

        var result = await _portal.GetMyPackagesAsync(userId, lang, now, ct);
        return Ok(result);
    }

    [HttpGet("p/{slug}")]
    [ProducesResponseType(typeof(PortalPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PortalPackageDto>> GetPackage(string slug, CancellationToken ct)
    {
        if (!TryGetUser(out var userId, out var lang))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var result = await _portal.GetPackageIfEntitledAsync(userId, slug, lang, ct);

        return result.Status switch
        {
            PortalAccessStatus.NotFound => NotFound(),
            PortalAccessStatus.Forbidden => Forbid(),
            PortalAccessStatus.Ok => Ok(result.Package),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    // -------------------------
    // Market-aware (ny)
    // -------------------------

    [HttpGet("{code}/my-packages")]
    [ProducesResponseType(typeof(IReadOnlyList<MyPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MyPackageDto>>> MyPackagesByMarket(string code, CancellationToken ct)
    {
        if (!TryGetUser(out var userId, out var lang))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(code))
            return NotFound();

        var now = DateTimeOffset.UtcNow;

        var result = await _portal.GetMyPackagesByMarketAsync(userId, code, lang, now, ct);
        return Ok(result);
    }

    [HttpGet("{code}/p/{slug}")]
    [ProducesResponseType(typeof(PortalPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PortalPackageDto>> GetPackageByMarket(string code, string slug, CancellationToken ct)
    {
        if (!TryGetUser(out var userId, out var lang))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var result = await _portal.GetPackageIfEntitledByMarketAsync(userId, code, slug, lang, ct);

        return result.Status switch
        {
            PortalAccessStatus.NotFound => NotFound(),
            PortalAccessStatus.Forbidden => Forbid(),
            PortalAccessStatus.Ok => Ok(result.Package),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    // -------------------------
    // Debug
    // -------------------------

    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping() => Ok("portal pong v3");

    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    private bool TryGetUser(out string userId, out string lang)
    {
        userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
              ?? User.FindFirstValue("sub")
              ?? string.Empty;

        var rawLang = User.FindFirstValue("lang") ?? "en";
        lang = rawLang.Trim().ToLowerInvariant() == "zh" ? "zh" : "en";

        return !string.IsNullOrWhiteSpace(userId);
    }
}
