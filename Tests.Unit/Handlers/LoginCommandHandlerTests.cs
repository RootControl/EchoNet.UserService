using Application.Commands;
using Application.DTOs;
using Application.Handlers;
using Application.Interfaces;
using Castle.Core.Configuration;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICreateJwtToken> _jwtTokenMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenMock = new Mock<ICreateJwtToken>();
        _handler = new LoginCommandHandler(_userRepositoryMock.Object, _jwtTokenMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnTokenResponse()
    {
        // Arrange
        var user = new User("testUser", "test@example.com", BCrypt.Net.BCrypt.HashPassword("password123"), RoleConstant.User);
        
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(user);
        
        var refreshToken = new TokenResponse("accessToken", "refreshToken", DateTime.Now.AddMinutes(30));
        
        _jwtTokenMock.Setup(x => x.CreateTokenAsync(It.IsAny<User>(), CancellationToken.None))
            .ReturnsAsync(refreshToken);

        var command = new LoginCommand(user.Email, "password123");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), CancellationToken.None), Times.Once);
        
        _jwtTokenMock.Verify(x => x.CreateTokenAsync(It.IsAny<User>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ThrowArgumentException()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), CancellationToken.None))!
            .ReturnsAsync((User)null!);

        var command = new LoginCommand("test@example.com", "password");
        
        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), CancellationToken.None), Times.Never);
    }
}