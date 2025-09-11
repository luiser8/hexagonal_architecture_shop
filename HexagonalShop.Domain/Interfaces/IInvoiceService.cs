using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Interfaces;

public interface IInvoiceService
{
    Task<Invoice?>? GetById(int id);
    Task Create(Invoice invoice);
    Task Delete(int id);
}