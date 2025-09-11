using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Ports;

namespace HexagonalShop.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    public ProductService(IProductRepository productRepository) => _productRepository = productRepository;
    
    public async Task<List<Product>> GetAll() => await _productRepository.GetAll();
    public async Task<Product?> GetById(int id) => await _productRepository.GetById(id);
    public async Task Create(Product product) => await _productRepository.Create(product);
    public async Task Update(int id, Product product) => await _productRepository.Update(id, product);
    public async Task Delete(int id) => await _productRepository.Delete(id);
    public async Task AddStock(int productId, int quantity) => await _productRepository.AddStock(productId, quantity);
}