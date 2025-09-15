using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Ports;

namespace HexagonalShop.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    public OrderService(IOrderRepository orderRepository) => _orderRepository = orderRepository;
    public async Task<int> Create(Order order) => await _orderRepository.Create(order);
    public async Task<List<Order>> GetAll() => await _orderRepository.GetAll();
    public async Task<Order?>? GetById(int id) => await _orderRepository.GetById(id);
    public async Task<List<Order>> GetByUser(int idUser) => await _orderRepository.GetByUser(idUser);
    public async Task<Order?> GetByIdempotencyKey(string idempotencyKey) => await _orderRepository.GetByIdempotencyKey(idempotencyKey);
    public async Task Delete(int id) => await _orderRepository.Delete(id);
}