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
}