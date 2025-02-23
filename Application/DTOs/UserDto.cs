using Domain.Entities;

namespace Application.DTOs;

public class UserDto(User user) 
{
    public Guid Id { get; set; } = user.Id;
    public string Username { get; set; } = user.Username;
    public string Email { get; set; } = user.Email;
    public string Role { get; set; } = user.Role;
}