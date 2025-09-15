using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;

namespace HexagonalShop.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IAuthRepository _authRepository;

    public AuthService(ITokenService tokenService, IAuthRepository authRepository, IPasswordHasher passwordHasher)
    {
        _tokenService = tokenService;
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<string?> Login(string email, string password)
    {
        var user = await _authRepository.Login(email);
        if (user == null || !_passwordHasher.VerifyPassword(password, user.Password)) return null;

        var token = _tokenService.GenerateToken(user);
        await _authRepository.SaveToken(new User
        {
            Id = user.Id,
            Token = token
        });

        return token;
    }

    public async Task<bool?> Logout(string token)
    {
        return await _authRepository.Logout(3);
    }
}