namespace Domain.Entities;

public class User(string username, string email, string passwordHash, string role)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Username { get; private set; } = username;
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;
    public string Role { get; private set; } = role;
    public string RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiry { get; private set; }
}