using Application.DTOs;
using MediatR;

namespace Application.Commands;

public record UpdateProfileCommand(Guid UserId, string Username, string Email) : IRequest<UserDto>;