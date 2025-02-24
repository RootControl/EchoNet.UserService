using Application.Commands;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Handlers;

public class RefreshTokenCommandHandler(IUserRepository userRepository, ICreateJwtToken jwtToken) : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICreateJwtToken _jwtToken = jwtToken;
    
    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByRefreshToken(request.RefreshToken, cancellationToken);
        
        if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var token = await _jwtToken.CreateTokenAsync(user, cancellationToken);
        
        user.SetRefreshToken(token.RefreshToken, token.TokenExpiry);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return token;
    }
}