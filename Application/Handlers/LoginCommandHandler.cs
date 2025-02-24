using Application.Commands;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Handlers;

public class LoginCommandHandler(IUserRepository userRepository, ICreateJwtToken jwtToken) : IRequestHandler<LoginCommand, TokenResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICreateJwtToken _jwtToken = jwtToken;
    
    public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        ValidatePassword(user, request.Password);
        
        var token = await _jwtToken.CreateTokenAsync(user, cancellationToken);
        
        user.SetRefreshToken(token.RefreshToken, token.TokenExpiry);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return token;
    }
    
    private static void ValidatePassword(User user, string requestPassword)
    {
        if (user is null)
            throw new ArgumentException("User not found");
        
        if (!BCrypt.Net.BCrypt.Verify(requestPassword, user.PasswordHash))
            throw new ArgumentException("Invalid credentials");
    }
}