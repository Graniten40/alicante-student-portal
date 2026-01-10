namespace StudentPortal.Api.Dtos;

public record RegisterDto(string Email, string Password, string DisplayName, string PreferredLanguage);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string Email, string DisplayName, string PreferredLanguage);
