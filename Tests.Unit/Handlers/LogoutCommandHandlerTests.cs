using Application.Commands;
using Application.Handlers;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Handlers;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new LogoutCommandHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUserId_ReturnUnit()
    {
        // Arrange
        var user = new User("testUser", "test@email.com", "password123", RoleConstant.User);
        var command = new LogoutCommand(user.Id);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(r => r.UpdateAsync(user, CancellationToken.None));
            
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<MediatR.Unit>();
        
        _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(user, CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task Handle_InvalidUserId_ThrowKeyNotFoundException()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((User)null!);
        
        var command = new LogoutCommand(Guid.NewGuid());
        
        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}