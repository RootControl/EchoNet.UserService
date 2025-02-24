namespace Domain.Entities;

public class User(string username, string email, string passwordHash, string role)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Username { get; private set; } = username;
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;
    public string Role { get; private set; } = role;
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiry { get; private set; }
    
    public void SetRefreshToken(string? refreshToken, DateTime expiry)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = expiry;
    }
    
    public void UpdateProfile(string username, string email)
    {
        Username = username;
        Email = email;
    }

    public void RemoveRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = default;
    }
}