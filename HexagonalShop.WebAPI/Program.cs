using System.Text.Json.Serialization;
using HexagonalShop.API.Middleware;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Services;
using HexagonalShop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using HexagonalShop.Application.UseCases;
using HexagonalShop.Infrastructure.Persistence;
using HexagonalShop.Infrastructure.Security;
using HexagonalShop.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddControllers(options =>
    {
        options.Filters.Add(
            new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
        options.Filters.Add(
            new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
        options.Filters.Add(
            new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
        options.Filters.Add(
            new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
        options.Filters.Add(
            new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerWithAuth();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins("*")
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                .WithExposedHeaders("X-Custom-Header");
        });
});
builder.Services.AddDataProtection();
builder.Services.AddDbContext<AppShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<OrderHandler>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserHandler>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<InvoiceHandler>();

var app = builder.Build();

app.UseSwaggerWithAuth();
app.UseCors(myAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<AuthMiddleware>();
app.UseMiddleware<SecurityHeaderMiddleware>();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
