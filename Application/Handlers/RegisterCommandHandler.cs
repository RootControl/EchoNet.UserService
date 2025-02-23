using Application.Commands;
using Application.DTOs;
using MediatR;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Handlers;

public class RegisterCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    
    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User(
            request.Username, 
            request.Email, 
            BCrypt.Net.BCrypt.HashPassword(request.Password), 
            "User");

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto(user);
    }
}