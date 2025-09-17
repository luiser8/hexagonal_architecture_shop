using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using HexagonalShop.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Controllers;

public class AuthControllerTest
{
    private readonly Mock<AuthHandler> _authMockHandler;
    private readonly AuthController _authController;

    public AuthControllerTest()
    {
        _authMockHandler = new Mock<AuthHandler>();
        _authController = new AuthController(_authMockHandler.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginDto = new AuthLoginDto { Email = "test@example.com", Password = "password123" };
        _authMockHandler.Setup(x => x.ExecuteLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("token");

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        Assert.IsType<OkResult>(result);
    }
}