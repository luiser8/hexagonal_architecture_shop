using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HexagonalShop.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppShopContext _appShopContext;
    public UserRepository(AppShopContext appShopContext) => _appShopContext = appShopContext;

    public async Task<List<User>> GetAll() => await _appShopContext.Users.ToListAsync();

    public async Task<User?> GetById(int id) => await _appShopContext.Users.Include(x => x.Orders).FirstOrDefaultAsync(x => x.Id == id);

    public async Task Create(User user)
    {
        _appShopContext.Users.Add(user);
        await _appShopContext.SaveChangesAsync();
    }

    public async Task Update(int id, User user)
    {
        var userToUpdate = await _appShopContext.Users.FindAsync(id);
        if(userToUpdate == null)
            throw new Exception("User not found");

        userToUpdate.Name = user.Name;
        userToUpdate.Email = user.Email;
        userToUpdate.Password = user.Password;
        userToUpdate.Token = user.Token;

        await _appShopContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var userToDelete = await _appShopContext.Users.FindAsync(id);
        if(userToDelete == null) throw new Exception("User not found");
        _appShopContext.Users.Remove(userToDelete);
        await _appShopContext.SaveChangesAsync();
    }
}