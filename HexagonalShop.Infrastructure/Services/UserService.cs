using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;

namespace HexagonalShop.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task<List<User>> GetAll() => await _userRepository.GetAll();
    public async Task<User?> GetById(int id) => await _userRepository.GetById(id);

    public async Task Create(User user)
    {
        user.Password = _passwordHasher.HashPassword(user.Password);
        await _userRepository.Create(user);
    }
    public async Task Update(int id, User user) => await _userRepository.Update(id, user);
    public async Task Delete(int id) => await _userRepository.Delete(id);
}