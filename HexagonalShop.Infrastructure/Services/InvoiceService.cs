using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.Domain.Ports;

namespace HexagonalShop.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    public InvoiceService(IInvoiceRepository invoiceRepository) => _invoiceRepository = invoiceRepository;
    
    public async Task<Invoice?>? GetById(int id) => await _invoiceRepository.GetById(id);
    public async Task Create(Invoice invoice) => await _invoiceRepository.Create(invoice);
    public async Task Delete(int id) => await _invoiceRepository.Delete(id);
}