using Application.Commands;
using Application.DTOs;
using Application.Handlers;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Handlers;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new RegisterCommandHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsUserAndReturnsDto()
    {
        // Arrange
        var user = new User("testUser", "test@email.com", "password123", RoleConstant.User);
        var command = new RegisterCommand(user.Username, user.Email, user.PasswordHash);
        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await _handler.Handle(command, cancellationToken);
        
        // Assert
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), cancellationToken), Times.Once);
        
        result.Should()
            .BeEquivalentTo(
                new UserDto(user), 
                options => options.Excluding(d => d.Id)
            );
    }
    
    [Fact]
    public async Task Handle_InvalidCommand_ThrowsException()
    {
        // Arrange
        var command = new RegisterCommand("", "", "");
        var cancellationToken = CancellationToken.None;
        
        // Act
        Func<Task> act = async () => await _handler.Handle(command, cancellationToken);
        
        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), cancellationToken), Times.Never);
    }
}