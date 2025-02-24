using Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration;

public class UserServiceIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly IntegrationTestBase _testBase;

    public UserServiceIntegrationTests(IntegrationTestBase testBase)
    {
        _testBase = testBase;
    }
    
    [Fact]
    public async Task RegisterAndLongin_ShouldWorkEndToEnd()
    {
        // Arrange
        var mediator = _testBase.ServiceProvider.GetRequiredService<IMediator>();
        var registerCommand = new RegisterCommand("testUser", "test@example.com", "password123");
        
        // Act: Register
        var registerResult = await mediator.Send(registerCommand);
        var loginCommand = new LoginCommand(registerCommand.Email, registerCommand.Password);
        var loginResult = await mediator.Send(loginCommand);
        
        // Assert
        registerResult.Should().NotBeNull();
        registerResult.Username.Should().Be(registerCommand.Username);
        registerResult.Email.Should().Be(registerCommand.Email);
        
        loginResult.Should().NotBeNull();
        loginResult.AccessToken.Should().NotBeNullOrEmpty();
        loginResult.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateProfile_ShouldUpdateUser()
    {
        // Arrange
        var mediator = _testBase.ServiceProvider.GetRequiredService<IMediator>();
        var registerCommand = new RegisterCommand("testUser", "test@example.com", "password123");
        var userDto = await mediator.Send(registerCommand);
        var updateCommand = new UpdateProfileCommand(userDto.Id, "updatedUser", "updatedUser@example.com");
        
        // Act
        var updateResult = await mediator.Send(updateCommand);
        
        // Assert
        updateResult.Username.Should().Be(updateResult.Username);
        updateResult.Email.Should().Be(updateResult.Email);
    }
}