using HexagonalShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HexagonalShop.Infrastructure.Persistence;
public class AppShopContext(DbContextOptions<AppShopContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();
        
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderDetails)
            .WithOne(od => od.Order)
            .HasForeignKey(od => od.IdOrder);
        
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(o => o.Orders)
            .HasForeignKey(o => o.IdUser);

        modelBuilder.Entity<OrderDetails>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.IdProduct);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Order)
            .WithMany(i => i.Invoices)
            .HasForeignKey(i => i.IdOrder);
    }
}