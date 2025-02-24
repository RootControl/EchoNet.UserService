using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface ICreateJwtToken
{
    Task<TokenResponse> CreateTokenAsync(User user, CancellationToken cancellationToken);
}