using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Auth;
using StudentPortal.Api.Data;
using StudentPortal.Api.Dtos;
using StudentPortal.Api.Services;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Consumes("application/json")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly TokenService _tokens;
    private readonly AppDbContext _db;

    public AuthController(
        UserManager<AppUser> users,
        SignInManager<AppUser> signIn,
        TokenService tokens,
        AppDbContext db)
    {
        _users = users;
        _signIn = signIn;
        _tokens = tokens;
        _db = db;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var lang = string.IsNullOrWhiteSpace(dto.PreferredLanguage)
            ? "en"
            : dto.PreferredLanguage.Trim().ToLowerInvariant();

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            PreferredLanguage = lang
        };

        var result = await _users.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new { code = e.Code, description = e.Description });
            return BadRequest(errors);
        }

        // ✅ Auto-grant Basic Arrival Pack
        var basic = await _db.Products.FirstOrDefaultAsync(p => p.Slug == "basic-arrival");
        if (basic != null)
        {
            var alreadyHas = await _db.Entitlements.AnyAsync(e => e.UserId == user.Id && e.ProductId == basic.Id);
            if (!alreadyHas)
            {
                _db.Entitlements.Add(new StudentPortal.Api.Models.Entitlement
                {
                    UserId = user.Id,
                    ProductId = basic.Id
                });

                await _db.SaveChangesAsync();
            }
        }

        var token = _tokens.CreateToken(user);
        return Ok(new AuthResponseDto(token, user.Email!, user.DisplayName, user.PreferredLanguage));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var result = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _tokens.CreateToken(user);
        return Ok(new AuthResponseDto(token, user.Email!, user.DisplayName, user.PreferredLanguage));
    }
}
