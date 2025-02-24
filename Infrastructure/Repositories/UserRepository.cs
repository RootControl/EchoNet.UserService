using MongoDB.Driver;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UserRepository(MongoDbContext context) : IUserRepository
{
    private readonly MongoDbContext _context = context;
    
    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: cancellationToken);
    }

    public async Task<User> GetUserByRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        return await _context.Users.Find(u => u.RefreshToken == refreshToken).FirstOrDefaultAsync(cancellationToken);
    }
}