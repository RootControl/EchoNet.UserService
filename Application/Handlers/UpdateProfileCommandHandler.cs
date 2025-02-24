using Application.Commands;
using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Handlers;

public class UpdateProfileCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    
    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user is null)
            throw new KeyNotFoundException("User not found.");
        
        user.UpdateProfile(request.Username, request.Email);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new UserDto(user);
    }
}