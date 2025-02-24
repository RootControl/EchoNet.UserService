using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task<User> GetUserByRefreshToken(string refreshToken, CancellationToken cancellationToken);
}