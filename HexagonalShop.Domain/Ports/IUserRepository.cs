using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Ports;

public interface IUserRepository
{
    Task<List<User>> GetAll();
    Task<User?> GetById(int id);
    Task Create(User user);
    Task Update(int id, User user);
    Task Delete(int id);
}