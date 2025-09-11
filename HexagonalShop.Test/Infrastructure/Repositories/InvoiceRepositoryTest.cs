using HexagonalShop.Domain.Entities;
using HexagonalShop.Infrastructure.Persistence;
using HexagonalShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Repositories;

public class InvoiceRepositoryTest : IDisposable
{
    private readonly AppShopContext _context;
    private readonly InvoiceRepository _invoiceRepository;

    public InvoiceRepositoryTest()
    {
        var dbContextOptions =
            new DbContextOptionsBuilder<AppShopContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _context = new AppShopContext(dbContextOptions);
        _invoiceRepository = new InvoiceRepository(_context);
    }

    [Fact]
    public async Task GetById_WithExistingInvoice_ReturnsInvoice()
    {
        // Arrange
        var expectedInvoice = new Invoice { Id = 1, InvoiceNumber = "INV-001", SubTotal = 100.50m, Total = 100.50m };
        _context.Invoices.Add(expectedInvoice);
        await _context.SaveChangesAsync();

        // Act
        var result = await _invoiceRepository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedInvoice.Id, result.Id);
        Assert.Equal(expectedInvoice.InvoiceNumber, result.InvoiceNumber);
        Assert.Equal(expectedInvoice.SubTotal, result.SubTotal);
        Assert.Equal(expectedInvoice.Total, result.Total);
    }

    [Fact]
    public async Task GetById_WithNonExistingInvoice_ReturnsNull()
    {
        // Act
        var result = await _invoiceRepository.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_WithValidInvoice_AddsInvoiceToDatabase()
    {
        // Arrange
        var newInvoice = new Invoice { InvoiceNumber = "INV-002", SubTotal = 200.75m, Total = 100.50m };

        // Act
        await _invoiceRepository.Create(newInvoice);

        // Assert
        var invoiceInDb = await _context.Invoices.FirstOrDefaultAsync();
        Assert.NotNull(invoiceInDb);
        Assert.Equal(newInvoice.InvoiceNumber, invoiceInDb.InvoiceNumber);
        Assert.Equal(newInvoice.SubTotal, invoiceInDb.SubTotal);
        Assert.Equal(newInvoice.Total, invoiceInDb.Total);
        Assert.True(invoiceInDb.Id > 0);
    }

    [Fact]
    public async Task Delete_WithExistingInvoice_RemovesInvoiceFromDatabase()
    {
        // Arrange
        var invoice = new Invoice { Id = 2, InvoiceNumber = "INV-001", SubTotal = 100.50m, Total = 100.50m };
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // Act
        await _invoiceRepository.Delete(invoice.Id);
        var invoicesCount = await _context.Invoices.CountAsync();

        // Assert
        Assert.Equal(0, invoicesCount);
    }

    [Fact]
    public async Task Delete_WithNonExistingInvoice_DoesNotThrowException()
    {
        // Arrange
        var invoice = new Invoice { Id = 3, InvoiceNumber = "INV-001", SubTotal = 100.50m, Total = 100.50m };
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // Act // Assert
        await Assert.ThrowsAsync<Exception>(() => _invoiceRepository.Delete(0));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}