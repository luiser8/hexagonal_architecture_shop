using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Interfaces;

public interface IAuthService
{
    Task <string?> Login(string email, string password);
    Task<bool?> Logout(string token);
}