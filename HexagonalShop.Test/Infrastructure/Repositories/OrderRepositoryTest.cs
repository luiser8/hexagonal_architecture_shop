using HexagonalShop.Domain.Entities;
using HexagonalShop.Infrastructure.Persistence;
using HexagonalShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Repositories;

public class OrderRepositoryTest : IDisposable
{
    private readonly AppShopContext _context;
    private readonly OrderRepository _orderRepository;

    public OrderRepositoryTest()
    {
        var dbContextOptions =
            new DbContextOptionsBuilder<AppShopContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _context = new AppShopContext(dbContextOptions);
        _orderRepository = new OrderRepository(_context);
    }

    [Fact]
    public async Task Create_WithValidOrder_AddsOrderToDatabase()
    {
        // Arrange
        var newOrder = new Order { Id = 1, IdempotencyKey = Guid.NewGuid().ToString(), IdUser = 1, Status = true, OrderDetails = [ new OrderDetails { IdProduct = 1, Quantity = 10} ] };

        // Act
        await _orderRepository.Create(newOrder);

        // Assert
        var ordersInDb = await _context.Orders.FirstOrDefaultAsync();
        Assert.NotNull(ordersInDb);
        Assert.Equal(newOrder.Id, ordersInDb.Id);
        Assert.Equal(newOrder.IdUser, ordersInDb.IdUser);
        Assert.True(ordersInDb.Id > 0);
    }


    [Fact]
    public async Task Delete_WithValidOrder_RemovesOrderFromDatabase()
    {
        // Arrange
        var newOrder = new Order { Id = 1, IdempotencyKey = Guid.NewGuid().ToString(), IdUser = 1, Status = true, OrderDetails = [ new OrderDetails { IdProduct = 1, Quantity = 10} ] };
        await _orderRepository.Create(newOrder);

        // Act
        await _orderRepository.Delete(newOrder.Id);

        // Assert
        var ordersInDb = await _context.Orders.FirstOrDefaultAsync();
        Assert.Null(ordersInDb);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}