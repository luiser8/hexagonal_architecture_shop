using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.WebAPI.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Middleware;

public class AuthMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<AuthMiddleware>> _loggerMock;
    private readonly AuthMiddleware _middleware;
    private readonly string _testSecretKey;

    public AuthMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _configMock = new Mock<IConfiguration>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<AuthMiddleware>>();
        _testSecretKey = "TestSecretKey123456789012345678901234567890";

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(x => x.Value).Returns(_testSecretKey);
        _configMock.Setup(x => x["Security:Token"]).Returns(_testSecretKey);

        _middleware = new AuthMiddleware(_nextMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_PublicPath_ShouldSkipAuthentication()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/auth/login";

        // Act
        await _middleware.InvokeAsync(context, _tokenServiceMock.Object, _loggerMock.Object);

        // Assert
        _tokenServiceMock.VerifyNoOtherCalls();
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_NoToken_ShouldReturn401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/protected";

        // Act
        await _middleware.InvokeAsync(context, _tokenServiceMock.Object, _loggerMock.Object);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ValidToken_ShouldCallNext()
    {
        // Arrange
        var token = GenerateTestToken(1);
        var context = CreateContextWithToken(token, "/api/protected");

        _tokenServiceMock.Setup(x => x.ValidateToken(1, token))
                        .ReturnsAsync(true);

        var nextCalled = false;
        _nextMock.Setup(x => x(It.IsAny<HttpContext>()))
                .Callback<HttpContext>(_ => nextCalled = true)
                .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context, _tokenServiceMock.Object, _loggerMock.Object);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_InvalidToken_ShouldReturn401()
    {
        // Arrange
        var context = CreateContextWithToken("invalid-token", "/api/protected");

        // Act
        await _middleware.InvokeAsync(context, _tokenServiceMock.Object, _loggerMock.Object);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_TokenWithoutUserId_ShouldReturn401()
    {
        // Arrange
        var tokenWithoutId = GenerateTestTokenWithoutId();
        var context = CreateContextWithToken(tokenWithoutId, "/api/protected");

        // Act
        await _middleware.InvokeAsync(context, _tokenServiceMock.Object, _loggerMock.Object);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_RevokedToken_ShouldReturn401()
    {
        // Arrange
        var token = GenerateTestToken(1);
        var context = CreateContextWithToken(token, "/api/protected");

        _tokenServiceMock.Setup(x => x.ValidateToken(1, token))
                        .ReturnsAsync(false);

        // Act
        await _middleware.InvokeAsync(context, _tokenServiceMock.Object, _loggerMock.Object);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_DifferentPublicPaths_ShouldSkipAuthentication()
    {
        // Arrange
        var publicPaths = new[]
        {
            "/api/auth",
            "/api/auth/login",
            "/api/user/register",
            "/api/user/register/confirm"
        };

        foreach (var path in publicPaths)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = path;

            // Act
            await _middleware.InvokeAsync(context, _tokenServiceMock.Object, _loggerMock.Object);

            // Assert
            Assert.Equal(200, context.Response.StatusCode);
        }
    }

    private string GenerateTestToken(int userId, DateTime? expiration = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_testSecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", userId.ToString())
            }),
            Expires = expiration ?? DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateTestTokenWithoutId()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_testSecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("name", "testuser")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private HttpContext CreateContextWithToken(string token, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.Headers.Authorization = $"Bearer {token}";
        return context;
    }
}