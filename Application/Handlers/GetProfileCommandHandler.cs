using Application.Commands;
using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Handlers;

public class GetProfileCommandHandler(IUserRepository userRepository) : IRequestHandler<GetProfileCommand, UserDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    
    public async Task<UserDto> Handle(GetProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user is null)
            throw new KeyNotFoundException("User not found");

        return new UserDto(user);
    }
}