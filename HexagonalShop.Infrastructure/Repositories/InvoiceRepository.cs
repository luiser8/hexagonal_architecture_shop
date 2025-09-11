using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Persistence;

namespace HexagonalShop.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppShopContext _appShopContext;
    public InvoiceRepository(AppShopContext appShopContext) => _appShopContext = appShopContext;
    
    public async Task<Invoice?>? GetById(int id) => await _appShopContext.Invoices.FindAsync(id);

    public async Task Create(Invoice invoice)
    {
        _appShopContext.Invoices.Add(invoice);
        await _appShopContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var invoiceToDelete = await _appShopContext.Invoices.FindAsync(id);
        if (invoiceToDelete == null) throw new Exception("Invoice not found");
        _appShopContext.Invoices.Remove(invoiceToDelete);
        await _appShopContext.SaveChangesAsync();
    }
}