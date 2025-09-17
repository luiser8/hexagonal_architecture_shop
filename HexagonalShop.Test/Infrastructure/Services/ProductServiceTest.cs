using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Services;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Services;

public class ProductServiceTest
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly ProductService _productService;

    public ProductServiceTest()
    {
        _productRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        };
        _productRepository.Setup(x => x.GetAll()).ReturnsAsync(products);

        // Act
        var result = await _productService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsProduct_return_SimpleProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Price = 10.0m, Stock = 100 };
        _productRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(product);

        // Act
        var result = await _productService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetById_ReturnsProduct_return_CompositeProduct()
    {
        // Arrange
        var compositeProduct = new Product
        {
            Id = 1,
            Name = "Composite Product",
            Price = 30.0m,
            Stock = 50
        };
        _productRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(compositeProduct);

        // Act
        var result = await _productService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Product>(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task Create_CallsRepository()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "New Product", Price = 15.0m, Stock = 150 };

        // Act
        await _productService.Create(product);

        // Assert
        _productRepository.Verify(x => x.Create(product), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Updated Product", Price = 20.0m, Stock = 200 };

        // Act
        await _productService.Update(1, product);

        // Assert
        _productRepository.Verify(x => x.Update(1, product), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsRepository()
    {
        // Act
        await _productService.Delete(1);

        // Assert
        _productRepository.Verify(x => x.Delete(1), Times.Once);
    }

    [Fact]
    public async Task AddStock_CallsRepository()
    {
        // Act
        await _productService.AddStock(1, 50);

        // Assert
        _productRepository.Verify(x => x.AddStock(1, 50), Times.Once);
    }
}