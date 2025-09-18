using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HexagonalShop.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductHandler _productHandler;
    public ProductController(ProductHandler productHandler) => _productHandler = productHandler;

    [HttpGet("all"), Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _productHandler.ExecuteAll());
    }

    [HttpGet, Authorize]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _productHandler.ExecuteById(id));
    }

    [HttpPost("add"), Authorize]
    public async Task<IActionResult> Post([FromBody] ProductDto product)
    {
        await _productHandler.ExecuteSave(product);
        return Ok("Product created successfully");
    }

    [HttpDelete("delete/{id}"), Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _productHandler.ExecuteDelete(id);
        return Ok("Product deleted successfully");
    }

    [HttpPut("update/{id}"), Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto product)
    {
        await _productHandler.ExecuteUpdate(id, product);
        return Ok("Product updated successfully");
    }

    [HttpPut("add-stock"), Authorize]
    public async Task<IActionResult> AddStock(int productId, int quantity)
    {
        await _productHandler.ExecuteAddStock(productId, quantity);
        return Ok("Stock added successfully");
    }
}