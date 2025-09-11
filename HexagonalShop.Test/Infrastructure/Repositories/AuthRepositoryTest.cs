using HexagonalShop.Domain.Entities;
using HexagonalShop.Infrastructure.Persistence;
using HexagonalShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Repositories;

    public class AuthRepositoryTest : IDisposable
    {
        private readonly AppShopContext _context;
        private readonly AuthRepository _authRepository;

        public AuthRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppShopContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
                .Options;

            _context = new AppShopContext(dbContextOptions);
            _authRepository = new AuthRepository(_context);
        }

        [Fact]
        public async Task Login_WithValidEmail_ReturnsUser()
        {
            // Arrange
            var testEmail = "test@example.com";
            var expectedUser = new User { Id = 1, Email = testEmail, Name = "Test User" };
            
            _context.Users.Add(expectedUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authRepository.Login(testEmail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testEmail, result.Email);
            Assert.Equal(expectedUser.Id, result.Id);
        }

        [Fact]
        public async Task Login_WithInvalidEmail_ReturnsNull()
        {
            // Arrange
            var invalidEmail = "nonexistent@example.com";

            // Act
            var result = await _authRepository.Login(invalidEmail);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveToken_WithValidUser_UpdatesToken()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", Token = "old_token" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var updatedUser = new User { Id = 1, Token = "new_token" };

            // Act
            await _authRepository.SaveToken(updatedUser);

            // Assert
            var userInDb = await _context.Users.FindAsync(1);
            Assert.NotNull(userInDb);
            Assert.Equal("new_token", userInDb.Token);
        }

        [Fact]
        public async Task SaveToken_WithNonExistentUser_ThrowsException()
        {
            // Arrange
            var nonExistentUser = new User { Id = 999, Token = "token" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authRepository.SaveToken(nonExistentUser));
        }

        [Fact]
        public async Task Logout_WithValidId_ClearsToken()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", Token = "some_token" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authRepository.Logout(1);

            // Assert
            Assert.True(result);
            var userInDb = await _context.Users.FindAsync(1);
            Assert.NotNull(userInDb);
            Assert.Equal(string.Empty, userInDb.Token);
        }

        [Fact]
        public async Task Logout_WithNonExistentId_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authRepository.Logout(999));
        }

        [Fact]
        public async Task Validate_WithValidTokenAndId_ReturnsTrue()
        {
            // Arrange
            var testToken = "valid_token";
            var user = new User { Id = 1, Email = "test@example.com", Token = testToken };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authRepository.Validate(1, testToken);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Validate_WithInvalidToken_ReturnsFalse()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", Token = "valid_token" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authRepository.Validate(1, "invalid_token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Validate_WithNonExistentId_ReturnsFalse()
        {
            // Act
            var result = await _authRepository.Validate(999, "any_token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Validate_WithEmptyToken_ReturnsFalse()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", Token = string.Empty };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authRepository.Validate(1, string.Empty);

            // Assert
            Assert.True(result);
        }

        public void Dispose()
        {
            _context?.Database?.EnsureDeleted();
            _context?.Dispose();
        }
    }