using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Services;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Services;

public class OrderServiceTest
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly OrderService _orderService;

    public OrderServiceTest()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _orderService = new OrderService(_orderRepository.Object);
    }

    [Fact]
    public async Task GetOrderById_ReturnsOrder()
    {
        // Arrange
        var orderId = 1;
        _orderRepository.Setup(x => x.GetById(orderId)).ReturnsAsync(new Order { Id = orderId });

        // Act
        var result = await _orderService.GetById(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task GetOrderByUser_ReturnsOrder()
    {
        // Arrange
        var userId = 1;
        _orderRepository.Setup(x => x.GetByUser(It.IsAny<int>())).ReturnsAsync([new() { IdUser = userId }]);

        // Act
        var result = await _orderService.GetByUser(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetOrderByIdempotencyId_ReturnsOrder()
    {
        // Arrange
        var idempotencyKey = "unique_key_123";
        _orderRepository.Setup(x => x.GetByIdempotencyKey(idempotencyKey)).ReturnsAsync(new Order { IdempotencyKey = idempotencyKey });

        // Act
        var result = await _orderService.GetByIdempotencyKey(idempotencyKey);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idempotencyKey, result.IdempotencyKey);
    }

    [Fact]
    public async Task GetOrderAll_ReturnsOrders()
    {
        // Arrange
        var orders = new List<Order>
      {
          new() { Id = 1 },
          new() { Id = 2 }
      };
        _orderRepository.Setup(x => x.GetAll()).ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CreateOrder_CreatesNewOrder()
    {
        // Arrange
        var orderPayload = new Order
        {
            Id = 2,
            IdUser = 1,
            IdempotencyKey = "unique_key_123",
            Status = true,
            SystemDate = DateTime.Now
        };
        _orderRepository.Setup(x => x.Create(It.IsAny<Order>())).ReturnsAsync(orderPayload.Id);
        _orderRepository.Setup(x => x.GetById(orderPayload.Id)).ReturnsAsync(orderPayload);

        // Act
        var createdOrderId = await _orderService.Create(orderPayload);
        var result = await _orderService.GetById(createdOrderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderPayload.Id, result.Id);
    }

    [Fact]
    public async Task DeleteOrder_ReturnsTrue()
    {
        // Arrange
        var orderId = 1;
        _orderRepository.Setup(x => x.Delete(It.IsAny<int>()));
        _orderRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((Order)null);

        // Act
        await _orderService.Delete(orderId);
        var result = await _orderService.GetById(orderId);

        // Assert
        Assert.Null(result);
    }
}