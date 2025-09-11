using HexagonalShop.Application.DTOs;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;

namespace HexagonalShop.Application.UseCases;

public class UserHandler
{
    private readonly IUserService _userService;
    public UserHandler(IUserService userService) => _userService = userService;
    public async Task<List<User>> ExecuteAll() => await _userService.GetAll();
    public async Task<User?> ExecuteById(int id) => await _userService.GetById(id);

    public async Task ExecuteSave(UserDto user)
    {
        await _userService.Create(new User
        {
            Name = user.Name,
            Email = user.Email,
            Password = user.Password,
            Token = string.Empty
        });
    } 
}