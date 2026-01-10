using Microsoft.AspNetCore.Identity;

namespace StudentPortal.Api.Auth;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = "";
    public string PreferredLanguage { get; set; } = "en"; // en/zh
}
