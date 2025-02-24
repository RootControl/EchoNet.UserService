namespace Application.DTOs;

public class TokenResponse(string accessToken, string? refreshToken, DateTime tokenExpiry)
{
    public string AccessToken { get; set; } = accessToken;
    public string? RefreshToken { get; set; } = refreshToken;
    public DateTime TokenExpiry { get; set; } = tokenExpiry;
}