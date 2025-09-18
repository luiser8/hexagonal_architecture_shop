using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Controllers;

public class ProductControllerTest
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductHandler _productHandler;
    private readonly ProductController _productController;

    public ProductControllerTest()
    {
        _productServiceMock = new Mock<IProductService>();
        _productHandler = new ProductHandler(_productServiceMock.Object);
        _productController = new ProductController(_productHandler);
    }

    [Fact]
    public async Task GetAllProduct_Existing_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var expectedProduct = new Product { Id = productId, Name = "Product 1", Price = 100 };
        _productServiceMock.Setup(x => x.GetAll()).ReturnsAsync([expectedProduct]);

        // Act
        var result = await _productController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(new[] { expectedProduct }, okResult.Value);
    }

    [Fact]
    public async Task GetProduct_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var expectedProduct = new Product { Id = productId, Name = "Product 1", Price = 100 };
        _productServiceMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(expectedProduct);

        // Act
        var result = await _productController.Get(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedProduct, okResult.Value);
    }

    [Fact]
    public async Task DeleteProduct_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        _productServiceMock.Setup(x => x.Delete(It.IsAny<int>()));

        // Act
        var result = await _productController.Delete(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Product deleted successfully", okResult.Value);
    }

    [Fact]
    public async Task CreateProduct_ReturnsOkResult()
    {
        // Arrange
        var expectedProduct = new ProductDto() {  Name = "Product 1", Price = 100, Stock = 10 };
        _productServiceMock.Setup(x => x.Create(It.IsAny<Product>()));

        // Act
        var result = await _productController.Post(expectedProduct);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Product created successfully", okResult.Value);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var expectedProduct = new ProductDto() {  Name = "Product 1", Price = 100, Stock = 10 };
        _productServiceMock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<Product>()));

        // Act
        var result = await _productController.Update(productId, expectedProduct);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Product updated successfully", okResult.Value);
    }

    [Fact]
    public async Task AddStockProduct_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var productStock = 10;
        _productServiceMock.Setup(x => x.AddStock(It.IsAny<int>(), It.IsAny<int>()));

        // Act
        var result = await _productController.AddStock(productId, productStock);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Stock added successfully", okResult.Value);
    }
}