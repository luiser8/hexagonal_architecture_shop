using HexagonalShop.Domain.Entities;
using HexagonalShop.Infrastructure.Persistence;
using HexagonalShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Repositories;

public class ProductRepositoryTest : IDisposable
{
    private readonly AppShopContext _context;
    private readonly ProductRepository _productRepository;

    public ProductRepositoryTest()
    {
        var dbContextOptions =
            new DbContextOptionsBuilder<AppShopContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _context = new AppShopContext(dbContextOptions);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public  async Task GetAll_WhenCalled_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Description = "Description 1", Stock = 10, Price = 100, Status = true },
            new Product { Id = 2, Name = "Product 2", Description = "Description 2", Stock = 20, Price = 200, Status = true }
        };
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Product 1");
        Assert.Contains(result, p => p.Name == "Product 2");
    }

    [Fact]
    public  async Task GetById_WhenCalled_ReturnsProduct()
    {
        // Arrange
        var products = new Product
            { Id = 1, Name = "Product 1", Description = "Description 1", Stock = 10, Price = 100, Status = true };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Product 1", result.Name);
        Assert.Equal("Description 1", result.Description);
    }

    [Fact]
    public async Task Create_WithValidProduct_AddsProductToDatabase()
    {
        // Arrange
        var newProduct = new Product { Id = 1, Name = "Test Product", Description = "Test Description", Stock = 10, Price = 100, Status = true };

        // Act
        await _productRepository.Create(newProduct);

        // Assert
        var productsInDb = await _context.Products.FirstOrDefaultAsync();
        Assert.NotNull(productsInDb);
        Assert.Equal(newProduct.Id, productsInDb.Id);
        Assert.Equal(newProduct.Name, productsInDb.Name);
        Assert.Equal(newProduct.Description, productsInDb.Description);
        Assert.Equal(newProduct.Stock, productsInDb.Stock);
        Assert.Equal(newProduct.Price, productsInDb.Price);
        Assert.Equal(newProduct.Status, productsInDb.Status);
    }

    [Fact]
    public async Task Update_WithValidProduct_UpdatesProductInDatabase()
    {
        // Arrange
        var newProduct = new Product { Id = 1, Name = "Test Product", Description = "Test Description", Stock = 10, Price = 100, Status = true };
        await _productRepository.Create(newProduct);
        var updatedProduct = new Product { Id = 1, Name = "Updated Product", Description = "Updated Description", Stock = 20, Price = 200, Status = false };

        // Act
        await _productRepository.Update(newProduct.Id, updatedProduct);

        // Assert
        var productsInDb = await _context.Products.FirstOrDefaultAsync();
        Assert.NotNull(productsInDb);
        Assert.Equal(newProduct.Id, productsInDb.Id);
        Assert.Equal(newProduct.Name, productsInDb.Name);
        Assert.Equal(newProduct.Description, productsInDb.Description);
        Assert.Equal(newProduct.Stock, productsInDb.Stock);
        Assert.Equal(newProduct.Price, productsInDb.Price);
        Assert.Equal(newProduct.Status, productsInDb.Status);
    }

    [Fact]
    public async Task AddStock_WithValidProduct_AddStockProductInDatabase()
    {
        // Arrange
        var newProduct = new Product { Id = 1, Name = "Test Product", Description = "Test Description", Stock = 10, Price = 100, Status = true };

        // Act
        await _productRepository.Create(newProduct);
        await _productRepository.AddStock(newProduct.Id, 5);

        // Assert
        var productsInDb = await _context.Products.FindAsync(newProduct.Id);

        Assert.NotNull(productsInDb);
        Assert.Equal(15, productsInDb.Stock);
    }

    [Fact]
    public async Task Delete_WithValidProduct_RemovesProductFromDatabase()
    {
        // Arrange
        var newProduct = new Product { Id = 1, Name = "Test Product", Description = "Test Description", Stock = 10, Price = 100, Status = true };
        await _productRepository.Create(newProduct);

        // Act
        await _productRepository.Delete(newProduct.Id);

        // Assert
        var productsInDb = await _context.Products.FirstOrDefaultAsync();
        Assert.Null(productsInDb);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}