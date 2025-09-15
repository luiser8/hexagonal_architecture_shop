using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Services;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Services;

public class AuthServiceTest
{
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IAuthRepository> _authRepository;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _passwordHasher = new Mock<IPasswordHasher>();
        _tokenService = new Mock<ITokenService>();
        _authRepository = new Mock<IAuthRepository>();
        _authService = new AuthService(_tokenService.Object, _authRepository.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task LoginAsync_returns_token_when_user_exist()
    {
        // Arrange
        var userEmail = "user@corre.com";
        var userPassword = "password";

        //Act
        _passwordHasher.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _tokenService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("fake_token");
        _authRepository.Setup(x => x.Login(It.IsAny<string>())).ReturnsAsync(new User { Id = 1, Password = "my_super_secret_key", Email = "user@corre.com", Token = string.Empty, Status = true});
        var userLogin = await _authService.Login(userEmail, userPassword);

        //Assert
        Assert.NotNull(userLogin);
    }

    [Fact]
    public async Task LoginAsync_returns_null_when_user_not_exist()
    {
        // Arrange
        var userEmail = "user@corre.com";
        var userPassword = "password";

        //Act
        _passwordHasher.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        _tokenService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("");
        _authRepository.Setup(x => x.Login(It.IsAny<string>())).ReturnsAsync((User)null);
        var userLogin = await _authService.Login(userEmail, userPassword);

        //Assert
        Assert.Null(userLogin);
    }

    [Fact]
    public async Task LogOutAsync_returns_token_when_user_exist()
    {
        // Arrange
        const string fakeToken = "fake_token";

        //Act
        _passwordHasher.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _tokenService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns(fakeToken);
        _authRepository.Setup(x => x.Logout(It.IsAny<int>())).ReturnsAsync(true);
        var userLogin = await _authService.Logout(fakeToken);

        //Assert
        Assert.NotNull(userLogin);
    }
}