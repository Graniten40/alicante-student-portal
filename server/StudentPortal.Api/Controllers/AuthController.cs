using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Api.Auth;
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

    public AuthController(UserManager<AppUser> users, SignInManager<AppUser> signIn, TokenService tokens)
    {
        _users = users;
        _signIn = signIn;
        _tokens = tokens;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        // Normalisera språk (bara "en" och "zh" om du vill hålla det strikt)
        var lang = string.IsNullOrWhiteSpace(dto.PreferredLanguage) ? "en" : dto.PreferredLanguage.Trim().ToLowerInvariant();

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
            // Returnera alltid JSON på samma form
            var errors = result.Errors.Select(e => new { code = e.Code, description = e.Description });
            return BadRequest(errors);
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
