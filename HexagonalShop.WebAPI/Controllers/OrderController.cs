using HexagonalShop.Application.UseCases;
using HexagonalShop.Application.DTOs;
using HexagonalShop.API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HexagonalShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderHandler _orderHandler;
    public OrderController(OrderHandler orderHandler) => _orderHandler = orderHandler;
    
    [HttpGet("all"), Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _orderHandler.ExecuteAll());
    }
    
    [HttpGet, Authorize]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _orderHandler.ExecuteById(id));
    }
    
    [HttpGet("byUser/{idUser}"), Authorize]
    public async Task<IActionResult> GetByUser(int idUser)
    {
        return Ok(await _orderHandler.ExecuteByUser(idUser));
    }
    
    [HttpPost("add-order")]
    public async Task<IActionResult> Post([FromBody] OrderDto order)
    {
        order.IdUser = Convert.ToInt32(IdentityMiddleware.Get(HttpContext.User.Identities));
        await _orderHandler.ExecuteSave(order);
        return Ok("Order created successfully");
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderHandler.ExecuteDelete(id);
        return Ok("Order deleted successfully");
    }
}