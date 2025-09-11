using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HexagonalShop.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppShopContext _appShopContext;
    public OrderRepository(AppShopContext appShopContext) => _appShopContext = appShopContext;
    
    public async Task<int> Create(Order order)
    {
        _appShopContext.Orders.Add(order);
        await _appShopContext.SaveChangesAsync();
        return order.Id;
    }

    public async Task Delete(int id)
    {
        var orderToDelete = await _appShopContext.Orders.FindAsync(id);
        if (orderToDelete == null) throw new Exception("Order not found");
        _appShopContext.Orders.Remove(orderToDelete);
        await _appShopContext.SaveChangesAsync();
    }
    public async Task<Order?> GetByIdempotencyKey(string idempotencyKey) =>
        await _appShopContext.Orders.FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey);

    public async Task<List<Order>> GetByUser(int idUser) => await _appShopContext.Orders.Where(x => x.IdUser == idUser && x.Status).ToListAsync();
    public async Task<Order?>? GetById(int id) => await _appShopContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
    public async Task<List<Order>>? GetAll() => await _appShopContext.Orders.ToListAsync();
}