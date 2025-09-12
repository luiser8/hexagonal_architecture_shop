using System.IdentityModel.Tokens.Jwt;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Security;

public class TokenServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _configMock = new Mock<IConfiguration>();

        // Configuración por defecto para el token secret
        _configMock.Setup(x => x["Security:Token"]).Returns("my_super_secret_key_that_is_long_enough_for_hmac_sha256");

        _tokenService = new TokenService(_authRepositoryMock.Object, _configMock.Object);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ReturnsValidJwtToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var token = _tokenService.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verificar que es un JWT válido
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.NotNull(jwtToken);
        Assert.Equal(4, jwtToken.Claims.Count()); // id, name, email

        // Verificar claims
        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
        Assert.NotNull(idClaim);
        Assert.Equal("1", idClaim.Value);

        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name");
        Assert.NotNull(nameClaim);
        Assert.Equal("John Doe", nameClaim.Value);

        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
        Assert.NotNull(emailClaim);
        Assert.Equal("john.doe@example.com", emailClaim.Value);

        // Verificar expiración (aproximadamente 2 horas en el futuro)
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow.AddHours(1.9));
        Assert.True(jwtToken.ValidTo < DateTime.UtcNow.AddHours(2.1));
    }

    [Fact]
    public async Task ValidateToken_WithValidIdAndToken_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var token = "valid_token";
        _authRepositoryMock.Setup(x => x.Validate(userId, token))
            .ReturnsAsync(true);

        // Act
        var result = await _tokenService.ValidateToken(userId, token);

        // Assert
        Assert.True(result);
        _authRepositoryMock.Verify(x => x.Validate(userId, token), Times.Once);
    }

    [Fact]
    public async Task ValidateToken_WithInvalidIdAndToken_ReturnsFalse()
    {
        // Arrange
        var userId = 999;
        var token = "invalid_token";
        _authRepositoryMock.Setup(x => x.Validate(userId, token))
            .ReturnsAsync(false);

        // Act
        var result = await _tokenService.ValidateToken(userId, token);

        // Assert
        Assert.False(result);
        _authRepositoryMock.Verify(x => x.Validate(userId, token), Times.Once);
    }

    [Fact]
    public async Task ValidateToken_WithEmptyToken_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var emptyToken = string.Empty;
        _authRepositoryMock.Setup(x => x.Validate(userId, emptyToken))
            .ReturnsAsync(false);

        // Act
        var result = await _tokenService.ValidateToken(userId, emptyToken);

        // Assert
        Assert.False(result);
        _authRepositoryMock.Verify(x => x.Validate(userId, emptyToken), Times.Once);
    }

    [Fact]
    public async Task ValidateToken_WithNegativeId_ReturnsFalse()
    {
        // Arrange
        var negativeId = -1;
        var token = "any_token";
        _authRepositoryMock.Setup(x => x.Validate(negativeId, token))
            .ReturnsAsync(false);

        // Act
        var result = await _tokenService.ValidateToken(negativeId, token);

        // Assert
        Assert.False(result);
        _authRepositoryMock.Verify(x => x.Validate(negativeId, token), Times.Once);
    }

    [Fact]
    public void GenerateToken_WithDifferentUsers_GeneratesDifferentTokens()
    {
        // Arrange
        var user1 = new User { Id = 1, Name = "User1", Email = "user1@example.com" };
        var user2 = new User { Id = 2, Name = "User2", Email = "user2@example.com" };

        // Act
        var token1 = _tokenService.GenerateToken(user1);
        var token2 = _tokenService.GenerateToken(user2);

        // Assert
        Assert.NotEqual(token1, token2);

        var handler = new JwtSecurityTokenHandler();
        var jwt1 = handler.ReadJwtToken(token1);
        var jwt2 = handler.ReadJwtToken(token2);

        Assert.Equal("1", jwt1.Claims.First(c => c.Type == "id").Value);
        Assert.Equal("2", jwt2.Claims.First(c => c.Type == "id").Value);
    }
}