using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Interfaces;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Application.UseCases;

public class AuthHandlerTest
{
  private readonly Mock<IAuthService> _authService;
private readonly AuthHandler _authHandler;

  public AuthHandlerTest()
  {
    _authService = new Mock<IAuthService>();
    _authHandler = new AuthHandler(_authService.Object);
  }

  [Fact]
  public async Task HandleLogin_ValidCredentials_ReturnsToken()
  {
    // Arrange
    var email = "user@example.com";
    var password = "secure-password";
    var token = "valid-token";
    _authService.Setup(x => x.Login(email, password)).ReturnsAsync(token);

    // Act
    var result = await _authHandler.ExecuteLogin(email, password);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(token, result);
  }

  [Fact]
  public async Task HandleLogin_InvalidCredentials_ReturnsNull()
  {
    // Arrange
    var email = "user@example.com";
    var password = "wrong-password";
    _authService.Setup(x => x.Login(email, password)).ReturnsAsync((string)null);

    // Act
    var result = await _authHandler.ExecuteLogin(email, password);

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public async Task HandleLogOut_InvalidCredentials_ReturnTrue()
  {
    // Arrange
    var token = "fake-token";
    _authService.Setup(x => x.Logout(token)).ReturnsAsync(true);

    // Act
    var result = await _authHandler.ExecuteLogout(token);

    // Assert
    Assert.True(result);
  }
}