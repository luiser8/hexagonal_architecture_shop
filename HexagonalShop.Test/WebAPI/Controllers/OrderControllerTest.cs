using System.Security.Claims;
using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Controllers;

public class OrderControllerTest
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly OrderHandler _orderHandler;
    private readonly OrderController _orderController;

    public OrderControllerTest()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _productServiceMock = new Mock<IProductService>();
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _orderHandler = new OrderHandler(_orderServiceMock.Object, _productServiceMock.Object, _invoiceServiceMock.Object);
        _orderController = new OrderController(_orderHandler)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetOrder_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var orderId = 1;
        var expectedOrder = new Order { Id = orderId, Status = true, OrderDetails = [new() { IdProduct = 1, Quantity = 2 }] };
        _orderServiceMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(expectedOrder);

        // Act
        var result = await _orderController.Get(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedOrder, okResult.Value);
    }

    [Fact]
    public async Task GetOrder_ExistingByUser_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedOrder = new List<Order> { new() { Id = 1, Status = true, OrderDetails = [new() { IdProduct = 1, Quantity = 2 }] } };
        _orderServiceMock.Setup(x => x.GetByUser(It.IsAny<int>())).ReturnsAsync(expectedOrder);

        // Act
        var result = await _orderController.GetByUser(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedOrder, okResult.Value);
    }

    [Fact]
    public async Task GetAllOrder_ExistingsList_ReturnsOkResult()
    {
        // Arrange
        var expectedOrders = new List<Order>
        {
            new() { Id = 1, Status = true, OrderDetails = [new() { IdProduct = 1, Quantity = 2 }] },
            new() { Id = 2, Status = true, OrderDetails = [new() { IdProduct = 2, Quantity = 1 }] }
        };
        _orderServiceMock.Setup(x => x.GetAll()).ReturnsAsync(expectedOrders);

        // Act
        var result = await _orderController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedOrders, okResult.Value);
    }

    [Fact]
    public async Task DeleteOrder_ExistingById_ReturnsOkResult()
    {
        // Arrange
        var orderId = 1;
        _orderServiceMock.Setup(x => x.Delete(It.IsAny<int>()));

        // Act
        var result = await _orderController.Delete(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Order deleted successfully", okResult.Value);
    }

    [Fact]
    public async Task CreateOrder_ReturnsOkResult()
    {
        // Arrange
        var orderDto = new OrderDto
        {
            IdUser = 1,
            IdempotencyKey = Guid.NewGuid().ToString(),
            OrderDetails = [new() { IdProduct = 1, Quantity = 2 }]
        };
        var claims = new[] { new Claim("userId", orderDto.IdUser.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _orderController.ControllerContext.HttpContext.User = principal;
        _orderServiceMock.Setup(x => x.GetByIdempotencyKey(It.IsAny<string>())).ReturnsAsync((Order)null);
        _productServiceMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(new Product
        {
            Id = 1,
            Name = "Product 1",
            Description = "Description 1",
            Price = 100.0m,
            Status = true,
            Stock = 10
        });
        _productServiceMock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<Product>()));
        _invoiceServiceMock.Setup(x => x.Create(It.IsAny<Invoice>()));
        _orderServiceMock.Setup(x => x.Create(It.IsAny<Order>())).ReturnsAsync(1);

        // Act
        var result = await _orderController.Post(orderDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Order created successfully", okResult.Value);
    }
}
