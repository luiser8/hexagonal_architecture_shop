using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Interfaces;

public interface IOrderService
{
    Task<List<Order>>? GetAll();
    Task<Order?>? GetById(int id);
    Task<int> Create(Order order);
    Task Delete(int id);
    Task<Order?>? GetByIdempotencyKey(string idempotencyKey);
    Task<List<Order>> GetByUser(int idUser);
}