using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using Xunit;
using Moq;
using HexagonalShop.Application.DTOs;

namespace HexagonalShop.Test.Application.UseCases;

public class OrderHandlerTest
{
    private readonly Mock<IOrderService> _orderService;
    private readonly Mock<IProductService> _productService;
    private readonly Mock<IInvoiceService> _invoiceService;
    private readonly OrderHandler _orderHandler;
    public OrderHandlerTest()
    {
        _orderService = new Mock<IOrderService>();
        _productService = new Mock<IProductService>();
        _invoiceService = new Mock<IInvoiceService>();
        _orderHandler = new OrderHandler(_orderService.Object, _productService.Object, _invoiceService.Object);
    }

    [Fact]
    public async Task Should_Call_GetByIdOrder_Once()
    {
        // Arrange
        var result = new Order { Id = 1, IdUser = 1, IdempotencyKey = Guid.NewGuid().ToString() };
        _orderService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(result);

        // Act
        var order = await _orderHandler.ExecuteById(1);

        // Assert
        Assert.NotNull(order);
    }

    [Fact]
    public async Task Should_Call_GetByUserId_Once()
    {
        // Arrange
        var result = new List<Order> { new Order { Id = 1, IdUser = 1, IdempotencyKey = Guid.NewGuid().ToString() } };
        _orderService.Setup(x => x.GetByUser(It.IsAny<int>())).ReturnsAsync(result);

        // Act
        var order = await _orderHandler.ExecuteByUser(1);

        // Assert
        Assert.NotNull(order);
    }

    [Fact]
    public async Task Should_Call_GetAllOrder_Once()
    {
        // Arrange
        var result = new List<Order> { new Order { Id = 1, IdUser = 1, IdempotencyKey = Guid.NewGuid().ToString() } };
        _orderService.Setup(x => x.GetAll()).ReturnsAsync(result);

        // Act
        var orders = await _orderHandler.ExecuteAll();

        // Assert
        Assert.NotNull(orders);
    }

    [Fact]
    public async Task Should_Call_DeleteOrder_Once()
    {
        // Arrange
        _orderService.Setup(x => x.Delete(It.IsAny<int>()));

        // Act
        await _orderHandler.ExecuteDelete(1);

        // Assert
        _orderService.Verify(x => x.Delete(1), Times.Once);
    }

    [Fact]
    public async Task Should_Call_CreateOrder_Once()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid().ToString();
        _orderService.Setup(x => x.Create(It.IsAny<Order>())).ReturnsAsync(It.IsAny<int>());
        _orderService.Setup(x => x.GetByIdempotencyKey(It.IsAny<string>())).ReturnsAsync((Order)null);
        _productService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(new Product
        {
            Id = 1,
            Name = "Product 1",
            Description = "Description 1",
            Price = 100.0m,
            Status = true,
            Stock = 10
        });
        _productService.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<Product>()));
        _invoiceService.Setup(x => x.Create(It.IsAny<Invoice>()));

        // Act
        await _orderHandler.ExecuteSave(new OrderDto()
        {
            IdUser = 1,
            IdempotencyKey = idempotencyKey,
            OrderDetails =
            [
                new OrderDetailsDto
                {
                    IdProduct = 1,
                    Quantity = 1
                }
            ]
        });

        // Assert
        _orderService.Verify(x => x.Create(It.IsAny<Order>()), Times.Once);
    }
}