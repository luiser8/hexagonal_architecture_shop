using HexagonalShop.Application.DTOs;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;

namespace HexagonalShop.Application.UseCases;

public class ProductHandler
{
    private readonly IProductService _productService;
    public ProductHandler(IProductService productService) => _productService = productService;
    
    public async Task<Product?>? ExecuteById(int id) => await _productService.GetById(id);
    public async Task<List<Product>>? ExecuteAll() => await _productService.GetAll();

    public async Task ExecuteSave(ProductDto product)
    {
        await _productService.Create(new Product
        {
            Name = product.Name,
            Description = product.Description,
            Stock = product.Stock,
            Price = product.Price,
            Status = product.Status,
        });
    } 
    public async Task ExecuteDelete(int id) => await _productService.Delete(id);

    public async Task ExecuteUpdate(int id, ProductDto product)
    {
        await _productService.Update(id, new Product
        {
            Id = id,
            Name = product.Name,
            Description = product.Description,
            Stock = product.Stock,
            Price = product.Price,
            Status = product.Status
        });
    }
    public async Task ExecuteAddStock(int productId, int quantity) => await _productService.AddStock(productId, quantity);
}