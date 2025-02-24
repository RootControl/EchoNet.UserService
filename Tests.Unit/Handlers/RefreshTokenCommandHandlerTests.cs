using Application.Commands;
using Application.DTOs;
using Application.Handlers;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Handlers;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<ICreateJwtToken> _jwtToken;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _jwtToken = new Mock<ICreateJwtToken>();
        _handler = new RefreshTokenCommandHandler(_userRepository.Object, _jwtToken.Object);
    }
    
    [Fact]
    public async Task Handle_WhenUserIsNull_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new RefreshTokenCommand("refreshToken");
        
        _userRepository.Setup(x => x.GetUserByRefreshToken(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        
        _userRepository.Verify(x => x.GetUserByRefreshToken(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_WhenRefreshTokenIsExpired_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = new User("testUser", "test@example.com", "password", RoleConstant.User);
        user.SetRefreshToken(string.Empty, DateTime.UtcNow.AddMinutes(-1));
        
        var request = new RefreshTokenCommand("refreshToken");
        
        _userRepository.Setup(x => x.GetUserByRefreshToken(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        
        _userRepository.Verify(x => x.GetUserByRefreshToken(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenIsValid_ReturnsTokenResponse()
    {
        // Arrange
        var user = new User("testUser", "test@example.com", "password", RoleConstant.User);
        user.SetRefreshToken(string.Empty, DateTime.UtcNow.AddMinutes(1));
        
        var request = new RefreshTokenCommand("refreshToken");
        var tokenResponse = new TokenResponse("accessToken", "refreshToken", DateTime.UtcNow.AddMinutes(1));
        
        _userRepository.Setup(x => x.GetUserByRefreshToken(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _jwtToken.Setup(x => x.CreateTokenAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        result.Should().BeEquivalentTo(tokenResponse);
        
        _userRepository.Verify(x => x.GetUserByRefreshToken(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _jwtToken.Verify(x => x.CreateTokenAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}