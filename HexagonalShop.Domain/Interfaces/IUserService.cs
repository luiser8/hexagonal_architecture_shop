using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Interfaces;

public interface IUserService
{
    public Task<List<User>> GetAll();
    public Task<User?> GetById(int id);
    public Task Create(User user);
    public Task Update(int id, User user);
    public Task Delete(int id);
}