using Application.DTOs;
using MediatR;

namespace Application.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse>;