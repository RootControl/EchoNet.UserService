using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class CreateJwtToken(IConfiguration configuration) : ICreateJwtToken
{
    private readonly IConfiguration _configuration = configuration;
    private readonly DateTime _tokenExpiry = DateTime.UtcNow.AddDays(7);
    private readonly DateTime _tokenDescriptionExpires = DateTime.UtcNow.AddMinutes(15);
    
    public Task<TokenResponse> CreateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = _tokenDescriptionExpires,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescription);
        var accessToken = tokenHandler.WriteToken(token);
        
        return Task.FromResult(new TokenResponse(
            accessToken, 
            Guid.NewGuid().ToString(),
            _tokenExpiry));
    }
}