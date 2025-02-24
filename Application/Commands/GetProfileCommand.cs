using Application.DTOs;
using MediatR;

namespace Application.Commands;

public record GetProfileCommand(Guid UserId) : IRequest<UserDto>;