using Application.Handlers;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Tests.Integration;

public class IntegrationTestBase : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();
    public IServiceProvider ServiceProvider { get; private set; }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
        
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MongoDb:ConnectionString", _mongoDbContainer.GetConnectionString() },
                { "MongoDb:DatabaseName", "TestDb" },
                { "Jwt:Key", "MIHcAgEBBEIBXbb7ECalXWFHhrs/Lm/GVIm7NR9zo8jWLJ4ydVTnyQgBwlkw8SrNOlsuw4bClr5cZLH5E/zzuBIwL33xJz/39I+gBwYFK4EEACOhgYkDgYYABAGA2vu/ls9+mdDGdbjsaF8kfv3foSu2HotTbhUL+xcA5w0ZgxV59IqskcrQZo3/6dps3aZzv80qcGuFgpIcMHPcvQBV3w+fFFGTsLeVk45hTd/WqNNB3p+oHFZp2I/NwrzCMn8UlMMgv9p3Gmsq7t2f+ejW4Ye5D+s/rRsvQnAVsK0IGA==" },
                { "Jwt:Issuer", "UserService" },
                { "Jwt:Audience", "UserService" }
            })
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICreateJwtToken, CreateJwtToken>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly));
        
        ServiceProvider = services.BuildServiceProvider();
        
        var context = ServiceProvider.GetRequiredService<MongoDbContext>();
        await context.Users.DeleteManyAsync(Builders<User>.Filter.Empty);
    }

    public async Task DisposeAsync() => await _mongoDbContainer.StopAsync();
}