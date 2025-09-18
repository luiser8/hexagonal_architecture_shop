using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Controllers;

public class UserControllerTest
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserHandler _userHandler;
    private readonly UserController _userController;

    public UserControllerTest()
    {
        _userServiceMock = new Mock<IUserService>();
        _userHandler = new UserHandler(_userServiceMock.Object);
        _userController = new UserController(_userHandler);
    }


    [Fact]
    public async Task GetAllUsers_Existing_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User { Id = userId, Name = "User 1", Email = "user1@example.com" };
        _userServiceMock.Setup(x => x.GetAll()).ReturnsAsync([expectedUser]);

        // Act
        var result = await _userController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(new[] { expectedUser }, okResult.Value);
    }

    [Fact]
    public async Task GetUser_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User { Id = userId, Name = "User 1", Email = "user1@example.com" };
        _userServiceMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(expectedUser);

        // Act
        var result = await _userController.GetById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedUser, okResult.Value);
    }

    [Fact]
    public async Task CreateUser_ValidUser_ReturnsCreatedResult()
    {
        // Arrange
        var expectedUser = new UserDto { Name = "User 1", Email = "user1@example.com", Password = "password" };
        _userServiceMock.Setup(x => x.Create(It.IsAny<User>()));

        // Act
        var result = await _userController.Post(expectedUser);

        // Assert
        var createdResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User created successfully", createdResult.Value);
    }
}