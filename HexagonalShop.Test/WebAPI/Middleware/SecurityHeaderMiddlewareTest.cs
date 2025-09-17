using HexagonalShop.WebAPI.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Middleware;

public class SecurityHeaderMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly SecurityHeaderMiddleware _middleware;

    public SecurityHeaderMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new SecurityHeaderMiddleware(_nextMock.Object);
    }

    [Fact]
    public async Task Invoke_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _middleware.Invoke(null!));
    }

    [Fact]
    public async Task Invoke_ShouldAddAllSecurityHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _nextMock.Setup(next => next.Invoke(context)).Returns(Task.CompletedTask);

        // Act
        await _middleware.Invoke(context);

        // Assert
        AssertSecurityHeaders(context.Response.Headers);
    }

    [Fact]
    public async Task Invoke_ShouldCallNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;
        _nextMock.Setup(next => next.Invoke(context))
                .Callback(() => nextCalled = true)
                .Returns(Task.CompletedTask);

        // Act
        await _middleware.Invoke(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task Invoke_ShouldAddHeadersInCorrectOrder()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _nextMock.Setup(next => next.Invoke(context)).Returns(Task.CompletedTask);

        // Act
        await _middleware.Invoke(context);

        // Assert
        var headers = context.Response.Headers;
        AssertSecurityHeaders(headers);
    }

    [Fact]
    public async Task Invoke_ShouldSetCorrectHeaderValues()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _nextMock.Setup(next => next.Invoke(context)).Returns(Task.CompletedTask);

        // Act
        await _middleware.Invoke(context);

        // Assert
        var headers = context.Response.Headers;

        Assert.Equal("DENY", headers["X-Frame-Options"].ToString());
        Assert.Equal("none", headers["X-Permitted-Cross-Domain-Policies"].ToString());
        Assert.Equal("1; mode=block", headers["X-Xss-Protection"].ToString());
        Assert.Equal("nosniff", headers["X-Content-Type-Options"].ToString());
        Assert.Equal("no-referrer", headers["Referrer-Policy"].ToString());
        Assert.Equal("default-src 'self'", headers["Content-Security-Policy"].ToString());
        Assert.Equal("no-cache, no-store", headers["Cache-Control"].ToString());
        Assert.Equal("camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()", 
                    headers["Permissions-Policy"].ToString());
        Assert.Equal("require-corp", headers["Cross-Origin-Embedder-Policy"].ToString());
        Assert.Equal("same-origin", headers["Cross-Origin-Resource-Policy"].ToString());
        Assert.Equal("same-origin", headers["Cross-Origin-Opener-Policy"].ToString());
        Assert.Equal("max-age=63072000; includeSubDomains; preload", 
                    headers["Strict-Transport-Security"].ToString());
    }

    private static void AssertSecurityHeaders(IHeaderDictionary headers)
    {
        var expectedHeaders = GetExpectedHeaders();

        foreach (var expectedHeader in expectedHeaders)
        {
            Assert.True(headers.ContainsKey(expectedHeader.Key),
                $"Missing header: {expectedHeader.Key}");

            Assert.Equal(expectedHeader.Value, headers[expectedHeader.Key].ToString());
        }
    }

    private static Dictionary<string, string> GetExpectedHeaders()
    {
        return new Dictionary<string, string>
        {
            ["X-Frame-Options"] = "DENY",
            ["X-Permitted-Cross-Domain-Policies"] = "none",
            ["X-Xss-Protection"] = "1; mode=block",
            ["X-Content-Type-Options"] = "nosniff",
            ["Referrer-Policy"] = "no-referrer",
            ["Content-Security-Policy"] = "default-src 'self'",
            ["Cache-Control"] = "no-cache, no-store",
            ["Permissions-Policy"] = "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()",
            ["Cross-Origin-Embedder-Policy"] = "require-corp",
            ["Cross-Origin-Resource-Policy"] = "same-origin",
            ["Cross-Origin-Opener-Policy"] = "same-origin",
            ["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains; preload"
        };
    }

    [Fact]
    public async Task Invoke_WithExceptionInNext_ShouldStillSetHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _nextMock.Setup(next => next.Invoke(context))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.Invoke(context));

        // Headers should still be set even if next middleware throws
        AssertSecurityHeaders(context.Response.Headers);
    }
}