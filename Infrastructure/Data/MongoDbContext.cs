using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        var client = new MongoClient(configuration["MongoDb:ConnectionString"]);
        _database = client.GetDatabase(configuration["MongoDb:DatabaseName"]);
        
        // Create indexes
        var users = Users;
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
        users.Indexes.CreateOne(new CreateIndexModel<User>(indexKeys));
    }
    
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    
    public async Task SeedAsync()
    {
        if (await Users.CountDocumentsAsync(_ => true) == 0)
        {
            var users = new[]
            {
                new User("admin1", "admin1@example.com", BCrypt.Net.BCrypt.HashPassword("admin123"), "admin"),
                new User("user1", "user1@example.com", BCrypt.Net.BCrypt.HashPassword("user123"), "user"),
                new User("user2", "user2@example.com", BCrypt.Net.BCrypt.HashPassword("user123"), "user"),
                new User("user3", "user3@example.com", BCrypt.Net.BCrypt.HashPassword("user123"), "user")
            };

            await Users.InsertManyAsync(users);
        }
    }
}