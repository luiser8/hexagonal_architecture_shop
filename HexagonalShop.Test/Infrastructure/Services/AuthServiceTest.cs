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

        //_authRepository.Setup(x => x.Login(It.IsAny<string>())).Returns(new User { Id = 1, Password = "my_super_secret_key", Email = "user@corre.com"});
        
        _authService = new AuthService(_tokenService.Object, _authRepository.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task LoginAsync_returns_token_when_user_exist()
    {
        // Arrange
        var userEmail = "user@corre.com";
        var userPassword = "password";
        
        //Act
        var userLogin = await _authService.Login(userEmail, userPassword);
        
        //Assert
        Assert.NotNull(userLogin);
    }
}