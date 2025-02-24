using Application.Commands;
using Domain.Interfaces;
using MediatR;

namespace Application.Handlers;

public class LogoutCommandHandler(IUserRepository userRepository) : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user is null)
            throw new KeyNotFoundException("User not found.");
        
        user.RemoveRefreshToken();

        await _userRepository.UpdateAsync(user, cancellationToken);
        return Unit.Value;
    }
}