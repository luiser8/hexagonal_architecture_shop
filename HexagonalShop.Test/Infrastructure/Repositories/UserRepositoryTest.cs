using HexagonalShop.Domain.Entities;
using HexagonalShop.Infrastructure.Persistence;
using HexagonalShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Repositories;

public class UserRepositoryTest : IDisposable
{
    private readonly AppShopContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTest()
    {
        var dbContextOptions =
            new DbContextOptionsBuilder<AppShopContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _context = new AppShopContext(dbContextOptions);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public  async Task GetAll_WhenCalled_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Name = "User 1", Email = "user1@example.com", Status = true },
            new User { Id = 2, Name = "User 2", Email = "user2@example.com", Status = true }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "User 1");
        Assert.Contains(result, p => p.Name == "User 2");
    }

    [Fact]
    public  async Task GetById_WhenCalled_ReturnsUser()
    {
        // Arrange
        var users = new User
            { Id = 1, Name = "User 1", Email = "user1@example.com", Status = true };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User 1", result.Name);
        Assert.Equal("user1@example.com", result.Email);
    }

    [Fact]
    public async Task Create_WithValidUser_AddsUserToDatabase()
    {
        // Arrange
        var newUser = new User { Id = 1, Name = "Test User", Email = "test@example.com", Password = "password" };

        // Act
        await _userRepository.Create(newUser);

        // Assert
        var usersInDb = await _context.Users.FirstOrDefaultAsync();
        Assert.NotNull(usersInDb);
        Assert.Equal(newUser.Id, usersInDb.Id);
        Assert.Equal(newUser.Name, usersInDb.Name);
        Assert.Equal(newUser.Email, usersInDb.Email);
        Assert.Equal(newUser.Password, usersInDb.Password);
    }

    [Fact]
    public async Task Update_WithValidUser_UpdatesUserInDatabase()
    {
        // Arrange
        var newUser = new User { Id = 1, Name = "Test User", Email = "test@example.com", Password = "password" };
        await _userRepository.Create(newUser);
        var updatedUser = new User { Id = 1, Name = "Updated User", Email = "updated@example.com", Password = "newpassword" };

        // Act
        await _userRepository.Update(newUser.Id, updatedUser);

        // Assert
        var usersInDb = await _context.Users.FirstOrDefaultAsync();
        Assert.NotNull(usersInDb);
        Assert.Equal(newUser.Id, usersInDb.Id);
        Assert.Equal(newUser.Name, usersInDb.Name);
        Assert.Equal(newUser.Email, usersInDb.Email);
        Assert.Equal(newUser.Password, usersInDb.Password);
    }

    [Fact]
    public async Task Delete_WithValidUser_RemovesUserFromDatabase()
    {
        // Arrange
        var newUser = new User { Id = 1, Name = "Test User", Email = "test@example.com", Password = "password" };
        await _userRepository.Create(newUser);

        // Act
        await _userRepository.Delete(newUser.Id);

        // Assert
        var usersInDb = await _context.Users.FirstOrDefaultAsync();
        Assert.Null(usersInDb);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}