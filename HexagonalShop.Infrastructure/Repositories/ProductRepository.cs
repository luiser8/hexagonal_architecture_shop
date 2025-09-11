using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HexagonalShop.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppShopContext _appShopContext;
    public ProductRepository(AppShopContext appShopContext) => _appShopContext = appShopContext;

    public async Task<List<Product>> GetAll() => await _appShopContext.Products.ToListAsync();

    public async Task<Product?> GetById(int id) => await _appShopContext.Products.FirstOrDefaultAsync(x => x.Id == id);

    public async Task Create(Product product)
    {
        _appShopContext.Products.Add(product);
        await _appShopContext.SaveChangesAsync();
    }

    public async Task Update(int id, Product product)
    {
        var productToUpdate = await _appShopContext.Products.FindAsync(id);
        if(productToUpdate == null)
            throw new Exception("Product not found");

        productToUpdate.Name = product.Name;
        productToUpdate.Description = product.Description;
        productToUpdate.Stock = product.Stock;
        productToUpdate.Price = product.Price;
        productToUpdate.Status = product.Status;
        
        await _appShopContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var productToDelete = await _appShopContext.Products.FindAsync(id);
        if(productToDelete == null)
            throw new Exception("Product not found");
        _appShopContext.Products.Remove(productToDelete);
        await _appShopContext.SaveChangesAsync();
    }
    public async Task AddStock(int productId, int quantity)
    {
        var productToAddStock = await _appShopContext.Products.FindAsync(productId);
        if(productToAddStock == null)
            throw new Exception("Product not found");
        
        productToAddStock.Stock += quantity;
        await _appShopContext.SaveChangesAsync();
    }
}