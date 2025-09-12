using HexagonalShop.Infrastructure.Security;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Security;

public class PasswordHasherTest
{
  private PasswordHasher _passwordHasher = new();

  [Fact]
  public void HashPassword_WithValidPassword_ReturnsHashedPassword()
  {
      // Arrange
      var password = "Test@123";

      // Act
      var hashedPassword = _passwordHasher.HashPassword(password);

      // Assert
      Assert.NotNull(hashedPassword);
      Assert.NotEqual(password, hashedPassword);
  }

  [Fact]
  public void VerifyPassword_WithMatchingPassword_ReturnsTrue()
  {
      // Arrange
      var password = "Test@123";
      var hashedPassword = _passwordHasher.HashPassword(password);

      // Act
      var result = _passwordHasher.VerifyPassword(password, hashedPassword);

      // Assert
      Assert.True(result);
  }

  [Fact]
  public void VerifyPassword_WithNonMatchingPassword_ReturnsFalse()
  {
      // Arrange
      var password = "Test@123";
      var hashedPassword = _passwordHasher.HashPassword(password);

      // Act
      var result = _passwordHasher.VerifyPassword("WrongPassword", hashedPassword);

      // Assert
      Assert.False(result);
  }
}