using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HexagonalShop.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _config;

    public TokenService(IAuthRepository authRepository,  IConfiguration config)
    {
        _authRepository = authRepository;
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new("id", user.Id.ToString()),
            new("name", user.Name),
            new("email", user.Email)
        };

        var tokenSecret = _config["Security:Token"] ?? string.Empty;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> ValidateToken(int id, string token)
    {
       return await _authRepository.Validate(id, token);
    }
}