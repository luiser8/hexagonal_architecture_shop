using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<bool> ValidateToken(int id, string token);
}
