using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Interfaces;

public interface IProductService
{
    public Task<List<Product>> GetAll();
    public Task<Product?> GetById(int id);
    public Task Create(Product product);
    public Task Update(int id, Product product);
    public Task Delete(int id);
    public Task AddStock(int productId, int quantity);
}