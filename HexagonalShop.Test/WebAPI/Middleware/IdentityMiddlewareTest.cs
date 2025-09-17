using System.Security.Claims;
using HexagonalShop.WebAPI.Middleware;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Middleware;

public class IdentityMiddlewareTests
{
    [Fact]
    public void Get_WithNullArgument_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<ClaimsIdentity>? nullIdentities = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => IdentityMiddleware.Get(nullIdentities!));
    }

    [Fact]
    public void Get_WithEmptyCollection_ShouldReturnEmptyString()
    {
        // Arrange
        var emptyIdentities = new List<ClaimsIdentity>();

        // Act
        var result = IdentityMiddleware.Get(emptyIdentities);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Get_WithIdentityHavingNoClaims_ShouldReturnEmptyString()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var identities = new List<ClaimsIdentity> { identity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Get_WithIdentityHavingEmptyClaims_ShouldReturnEmptyString()
    {
        // Arrange
        var identity = new ClaimsIdentity([]);
        var identities = new List<ClaimsIdentity> { identity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Get_WithIdentityHavingEmptyClaimValue_ShouldReturnEmptyString()
    {
        // Arrange
        var claim = new Claim("test", "");
        var identity = new ClaimsIdentity(new[] { claim });
        var identities = new List<ClaimsIdentity> { identity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Get_WithIdentityHavingValidClaimValue_ShouldReturnClaimValue()
    {
        // Arrange
        var expectedValue = "12345";
        var claim = new Claim("id", expectedValue);
        var identity = new ClaimsIdentity(new[] { claim });
        var identities = new List<ClaimsIdentity> { identity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Get_WithMultipleIdentities_ShouldUseFirstIdentity()
    {
        // Arrange
        var firstClaim = new Claim("id", "first");
        var secondClaim = new Claim("id", "second");

        var firstIdentity = new ClaimsIdentity(new[] { firstClaim });
        var secondIdentity = new ClaimsIdentity(new[] { secondClaim });

        var identities = new List<ClaimsIdentity> { firstIdentity, secondIdentity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal("first", result);
    }

    [Fact]
    public void Get_WithMultipleClaimsInIdentity_ShouldUseFirstClaim()
    {
        // Arrange
        var firstClaim = new Claim("id", "firstValue");
        var secondClaim = new Claim("name", "secondValue");

        var identity = new ClaimsIdentity(new[] { firstClaim, secondClaim });
        var identities = new List<ClaimsIdentity> { identity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal("firstValue", result);
    }

    [Fact]
    public void Get_WithWhitespaceClaimValue_ShouldReturnWhitespace()
    {
        // Arrange
        var claim = new Claim("id", "   ");
        var identity = new ClaimsIdentity(new[] { claim });
        var identities = new List<ClaimsIdentity> { identity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal("   ", result);
    }

    [Fact]
    public void Get_WithComplexClaimValue_ShouldReturnExactValue()
    {
        // Arrange
        var complexValue = "user-123@example.com";
        var claim = new Claim("id", complexValue);
        var identity = new ClaimsIdentity(new[] { claim });
        var identities = new List<ClaimsIdentity> { identity };

        // Act
        var result = IdentityMiddleware.Get(identities);

        // Assert
        Assert.Equal(complexValue, result);
    }
}