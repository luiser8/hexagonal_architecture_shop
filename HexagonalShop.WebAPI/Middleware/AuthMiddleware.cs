using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using HexagonalShop.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HexagonalShop.WebAPI.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    private readonly string[] _publicPath =
    [
        "/api/auth",
        "/api/user/register"
    ];

    public AuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService, ILogger<AuthMiddleware> logger)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        if (_publicPath.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        var jwtToken = authHeader?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(jwtToken))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token missing");
            return;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Security:Token"] ?? string.Empty);

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            var principal = tokenHandler.ValidateToken(jwtToken, validations, out _);
            context.User = principal;

            var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("User identity not found");
                return;
            }

            var isTokenValid = await tokenService.ValidateToken(Convert.ToInt32(userId), jwtToken);
            if (!isTokenValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token revoked");
                return;
            }
        }
        catch (SecurityTokenExpiredException)
        {
            context.Response.Headers.Append("Token-Expired", "true");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = 401,
                Message = "Token expired"
            }));
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating token");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = 401,
                Message = "Invalid or unauthorized token",
                Error = ex.Message
            }));
            return;
        }

        await _next(context);
    }
}
