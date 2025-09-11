using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HexagonalShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserHandler _userHandler;
    public UserController(UserHandler userHandler) => _userHandler = userHandler;

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _userHandler.ExecuteAll());
    }
    
    [HttpGet, Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _userHandler.ExecuteById(id));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Post([FromBody] UserDto user)
    {
        await _userHandler.ExecuteSave(user);
        return Ok("User created successfully");
    }
}