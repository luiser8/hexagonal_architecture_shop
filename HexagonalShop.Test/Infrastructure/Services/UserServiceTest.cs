using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Services;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Services;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userRepository = new Mock<IUserRepository>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _userService = new UserService(_userRepository.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        };
        _userRepository.Setup(x => x.GetAll()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsUserSimple()
    {
        // Arrange
        var user = new User { Id = 1 };
        _userRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(user);

        // Act
        var result = await _userService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task CreateUser_ReturnsUserSimple()
    {
        // Arrange
        var user = new User { Id = 1 };
        _userRepository.Setup(x => x.Create(It.IsAny<User>()));
        _userRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashedPassword");

        // Act
        await _userService.Create(user);
        var result = await _userService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task UpdateUser_ReturnsUserSimple()
    {
        // Arrange
        var user = new User { Id = 1 };
        _userRepository.Setup(x => x.Update(It.IsAny<int>(),It.IsAny<User>()));
        _userRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(user);

        // Act
        await _userService.Update(1, user);
        var result = await _userService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task DeleteUser_ReturnsUserSimple()
    {
        // Arrange
        _userRepository.Setup(x => x.Delete(It.IsAny<int>()));
        _userRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((User)null);

        // Act
        await _userService.Delete(1);
        var result = await _userService.GetById(1);

        // Assert
        Assert.Null(result);
    }
}