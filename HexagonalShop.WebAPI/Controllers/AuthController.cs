using HexagonalShop.Application.DTOs;
using HexagonalShop.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace HexagonalShop.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthHandler _authHandler;
    public AuthController(AuthHandler authHandler) => _authHandler = authHandler;

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthLoginDto dto)
    {
        var userSession = await _authHandler.ExecuteLogin(dto.Email, dto.Password);
        if (userSession == null)
            return Unauthorized();
        return Ok(userSession);
    }

    [HttpPut("logout/{token}")]
    public async Task<IActionResult> Logout(string token)
    {
        return Ok(await _authHandler.ExecuteLogout(token));
    }
}