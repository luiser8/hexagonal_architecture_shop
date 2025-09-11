using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Ports;

public interface IProductRepository
{
    Task<List<Product>> GetAll();
    Task<Product?> GetById(int id);
    Task Create(Product product);
    Task Update(int id, Product product);
    Task Delete(int id);
    Task AddStock(int productId, int quantity);
}