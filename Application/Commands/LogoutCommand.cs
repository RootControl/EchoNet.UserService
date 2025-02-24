using MediatR;

namespace Application.Commands;

public record LogoutCommand(Guid UserId) : IRequest<Unit>;