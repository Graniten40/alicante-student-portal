using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Api.Auth;
using StudentPortal.Api.Dtos;
using StudentPortal.Api.Services;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/auth")]
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
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            PreferredLanguage = string.IsNullOrWhiteSpace(dto.PreferredLanguage) ? "en" : dto.PreferredLanguage
        };

        var result = await _users.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        var token = _tokens.CreateToken(user);
        return Ok(new AuthResponseDto(token, user.Email!, user.DisplayName, user.PreferredLanguage));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        var result = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded) return Unauthorized("Invalid credentials");

        var token = _tokens.CreateToken(user);
        return Ok(new AuthResponseDto(token, user.Email!, user.DisplayName, user.PreferredLanguage));
    }
}
