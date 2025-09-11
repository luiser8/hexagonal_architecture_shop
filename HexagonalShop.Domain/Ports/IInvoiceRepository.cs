using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Domain.Ports;

public interface IInvoiceRepository
{
    Task<Invoice?>? GetById(int id);
    Task Create(Invoice invoice);
    Task Delete(int id);
}