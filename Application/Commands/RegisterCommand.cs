using Application.DTOs;
using MediatR;

namespace Application.Commands;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<UserDto>;