using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using Xunit;
using Moq;
using HexagonalShop.Application.DTOs;

namespace HexagonalShop.Test.Application.UseCases;

public class UserHandlerTest
{
  private readonly Mock<IUserService> _userService;
  private readonly UserHandler _userHandler;

  public UserHandlerTest()
  {
      _userService = new Mock<IUserService>();
      _userHandler = new UserHandler(_userService.Object);
  }

    [Fact]
    public async Task Should_Call_GetAllUsers_Once()
    {
        // Arrange
        var result = new List<User> { new() { Id = 1, Name = "User 1" } };
        _userService.Setup(x => x.GetAll()).ReturnsAsync(result);

        // Act
        var users = await _userHandler.ExecuteAll();

        // Assert
        Assert.NotNull(users);
    }

    [Fact]
    public async Task Should_Call_GetByIdUser_Once()
    {
        // Arrange
        var result = new User { Id = 1, Name = "User 1" };
        _userService.Setup(x => x.GetById(1)).ReturnsAsync(result);

        // Act
        var user = await _userHandler.ExecuteById(1);

        // Assert
        Assert.NotNull(user);
    }

    [Fact]
    public async Task Should_Call_CreateUser_Once()
    {
        // Arrange
        var userDto = new UserDto
        {
            Name = "User 1",
            Email = "user1@example.com",
            Password = "Password1",
            Status = true
        };

        // Act
        await _userHandler.ExecuteSave(userDto);

        // Assert
        _userService.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
    }
}
