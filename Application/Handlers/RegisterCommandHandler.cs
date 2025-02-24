using Application.Commands;
using Application.DTOs;
using Domain.Constants;
using MediatR;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Handlers;

public class RegisterCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    
    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        ValidateCommand(request);
        
        var user = new User(
            request.Username, 
            request.Email, 
            BCrypt.Net.BCrypt.HashPassword(request.Password), 
            RoleConstant.User);

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto(user);
    }
    
    private static void ValidateCommand(RegisterCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Username) || 
            string.IsNullOrWhiteSpace(command.Email) || 
            string.IsNullOrWhiteSpace(command.Password))
        {
            throw new ArgumentException("Username, email, and password are required");
        }
    }
}