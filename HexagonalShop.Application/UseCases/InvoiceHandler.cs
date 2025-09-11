using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;

namespace HexagonalShop.Application.UseCases;

public class InvoiceHandler
{
    private readonly IInvoiceService _invoiceService;
    public InvoiceHandler(IInvoiceService invoiceService) => _invoiceService = invoiceService;

    public async Task<Invoice?> ExecuteById(int id) => await _invoiceService.GetById(id);
    public async Task ExecuteDelete(int id) => await _invoiceService.Delete(id);
}