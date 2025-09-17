using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Entities;
using Moq;
using Xunit;
using HexagonalShop.Application.DTOs;

namespace HexagonalShop.Test.Application.UseCases;

public class ProductHandlerTest
{
    private readonly Mock<IProductService> _productService;
    private readonly ProductHandler _productHandler;

    public ProductHandlerTest()
    {
        _productService = new Mock<IProductService>();
        _productHandler = new ProductHandler(_productService.Object);
    }

    [Fact]
    public async Task Should_Call_GetAllProducts_Once()
    {
        // Arrange
        var result = new List<Product> { new() { Id = 1, Name = "Product 1" } };
        _productService.Setup(x => x.GetAll()).ReturnsAsync(result);

        // Act
        var products = await _productHandler.ExecuteAll();

        // Assert
        Assert.NotNull(products);
    }

    [Fact]
    public async Task Should_Call_GetByIdProduct_Once()
    {
        // Arrange
        var result = new Product { Id = 1, Name = "Product 1" };
        _productService.Setup(x => x.GetById(1)).ReturnsAsync(result);

        // Act
        var products = await _productHandler.ExecuteById(1);

        // Assert
        Assert.NotNull(products);
    }

    [Fact]
    public async Task Should_Call_CreateProduct_Once()
    {
        // Arrange
        var productDto = new ProductDto
        {
            Name = "Product 1",
            Description = "Description 1",
            Stock = 10,
            Price = 100,
            Status = true
        };

        // Act
        await _productHandler.ExecuteSave(productDto);

        // Assert
        _productService.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Should_Call_DeleteProduct_Once()
    {
        // Arrange
        var productId = 1;

        // Act
        await _productHandler.ExecuteDelete(productId);

        // Assert
        _productService.Verify(x => x.Delete(productId), Times.Once);
    }

    [Fact]
    public async Task Should_Call_UpdateProduct_Once()
    {
        // Arrange
        var productId = 1;
        var productDto = new ProductDto
        {
            Name = "Product 1",
            Description = "Description 1",
            Stock = 10,
            Price = 100,
            Status = true
        };

        // Act
        await _productHandler.ExecuteUpdate(productId, productDto);

        // Assert
        _productService.Verify(x => x.Update(It.IsAny<int>(), It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Should_Call_AddStockProduct_Once()
    {
        // Arrange
        var productId = 1;
        var productQuantity = 5;

        // Act
        await _productHandler.ExecuteAddStock(productId, productQuantity);

        // Assert
        _productService.Verify(x => x.AddStock(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }
}
