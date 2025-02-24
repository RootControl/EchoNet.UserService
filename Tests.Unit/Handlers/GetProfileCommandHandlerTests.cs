using Application.Commands;
using Application.DTOs;
using Application.Handlers;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Handlers;

public class GetProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetProfileCommandHandler _handler;

    public GetProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetProfileCommandHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUserId_ReturnUserDto()
    {
        // Arrange
        var user = new User("testUser", "test@example.com", "password123", RoleConstant.User);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(user);

        var command = new GetProfileCommand(Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().BeEquivalentTo(new UserDto(user));
        
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task Handle_InvalidUserId_ThrowKeyNotFoundException()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((User)null!);
        
        var command = new GetProfileCommand(Guid.NewGuid());
        
        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}