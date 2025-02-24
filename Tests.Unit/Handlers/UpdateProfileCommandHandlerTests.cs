using Application.Commands;
using Application.DTOs;
using Application.Handlers;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Handlers;

public class UpdateProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly UpdateProfileCommandHandler _handler;

    public UpdateProfileCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _handler = new UpdateProfileCommandHandler(_userRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldUpdateProfile()
    {
        // Arrange
        var user = new User("testUsername", "test@example.com", "password", RoleConstant.User);
        var command = new UpdateProfileCommand(user.Id, user.Username, user.Email);
        
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, CancellationToken.None)).ReturnsAsync(user);
        _userRepository.Setup(x => x.UpdateAsync(user, CancellationToken.None));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new UserDto(user));
        
        _userRepository.Verify(x => x.GetByIdAsync(user.Id, CancellationToken.None), Times.Once);
        _userRepository.Verify(x => x.UpdateAsync(user, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var user = new User("testUsername", "test@example.com", "password", RoleConstant.User);
        var command = new UpdateProfileCommand(user.Id, user.Username, user.Email);
        
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, CancellationToken.None)).ReturnsAsync((User)null!);
        
        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        
        _userRepository.Verify(x => x.GetByIdAsync(user.Id, CancellationToken.None), Times.Once);
    }
}