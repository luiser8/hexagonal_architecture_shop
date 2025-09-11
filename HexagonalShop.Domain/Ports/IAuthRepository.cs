using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Ports;

public interface IAuthRepository
{
    Task <User?> Login(string email);
    Task SaveToken(User user);
    Task<bool> Logout(int id);
    Task<bool> Validate(int id, string token);
}