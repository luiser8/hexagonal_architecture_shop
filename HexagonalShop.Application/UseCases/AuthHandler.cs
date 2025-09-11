using HexagonalShop.Domain.Interfaces;

namespace HexagonalShop.Application.UseCases;

public class AuthHandler
{
    private readonly IAuthService _authService;
    public AuthHandler(IAuthService authService) => _authService = authService;

    public async Task<string?> ExecuteLogin(string username, string password) => await _authService.Login(username, password);
    public async Task<bool?> ExecuteLogout(string token) => await _authService.Logout(token);
}