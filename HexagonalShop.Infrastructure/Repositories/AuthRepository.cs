using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HexagonalShop.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppShopContext _appShopContext;
    public AuthRepository(AppShopContext appShopContext) => _appShopContext = appShopContext;
    
    public async Task<User?> Login(string email)
    {
        return await _appShopContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task SaveToken(User user)
    {
        var userToUpdate = await _appShopContext.Users.FindAsync(user.Id);
        if(userToUpdate == null)
            throw new Exception("User not found");

        userToUpdate.Id = user.Id;
        userToUpdate.Token = user.Token;
        
        await _appShopContext.SaveChangesAsync();
    }

    public async Task<bool> Logout(int id)
    {
        var user = await _appShopContext.Users.FindAsync(1);
        if(user == null)
            throw new Exception("User not found");

        user.Token = string.Empty;
        
        await _appShopContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> Validate(int id, string token)
    {
        var user = await _appShopContext.Users.FirstOrDefaultAsync(x => x.Token == token && x.Id == id);
        return user != null;
    }
}