using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentPortal.Api.Auth;
using StudentPortal.Api.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentPortal.Api.Services;

public class JwtTokenService
{
    private readonly JwtOptions _opt;
    private readonly UserManager<AppUser> _users;

    public JwtTokenService(IOptions<JwtOptions> opt, UserManager<AppUser> users)
    {
        _opt = opt.Value;
        _users = users;
    }

    public async Task<string> CreateTokenAsync(AppUser user)
    {
        var lang = (user.PreferredLanguage?.ToLowerInvariant() == "zh") ? "zh" : "en";

        var claims = new List<Claim>
        {
            // Bra att ha både sub + NameIdentifier
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),

            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new("displayName", user.DisplayName ?? ""),
            new("lang", lang),
        };

        // ✅ Lägg till roller i JWT (det som krävs för [Authorize(Roles="Admin")])
        var roles = await _users.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_opt.ExpMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
