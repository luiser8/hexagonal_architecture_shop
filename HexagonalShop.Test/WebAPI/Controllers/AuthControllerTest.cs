using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Controllers;

public class AuthControllerTest
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthHandler _authHandler;
    private readonly AuthController _authController;

    public AuthControllerTest()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authHandler = new AuthHandler(_authServiceMock.Object);
        _authController = new AuthController(_authHandler);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginDto = new AuthLoginDto { Email = "test@example.com", Password = "password123" };
        var expectedToken = "valid-token-123";
        _authServiceMock.Setup(x => x.Login(loginDto.Email, loginDto.Password))
                       .ReturnsAsync(expectedToken);

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedToken, okResult.Value);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new AuthLoginDto { Email = "test@example.com", Password = "wrongpassword" };
        _authServiceMock.Setup(x => x.Login(loginDto.Email, loginDto.Password))
                       .ReturnsAsync((string?)null);

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Logout_ValidToken_ReturnsOkWithTrue()
    {
        // Arrange
        var token = "valid-token";
        _authServiceMock.Setup(x => x.Logout(token))
                       .ReturnsAsync(true);

        // Act
        var result = await _authController.Logout(token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool?)okResult.Value);
    }

    [Fact]
    public async Task Logout_InvalidToken_ReturnsOkWithFalse()
    {
        // Arrange
        var token = "invalid-token";
        _authServiceMock.Setup(x => x.Logout(token))
                       .ReturnsAsync(false);

        // Act
        var result = await _authController.Logout(token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.False((bool?)okResult.Value);
    }
}