using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Ports;
using HexagonalShop.Infrastructure.Services;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Infrastructure.Services;

public class InvoiceServiceTest
{
    private readonly Mock<IInvoiceRepository> _invoiceRepository;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTest()
    {
        _invoiceRepository = new Mock<IInvoiceRepository>();
        _invoiceService = new InvoiceService(_invoiceRepository.Object);
    }

    [Fact]
    public async Task GetInvoices_By_Id_return_Invoice()
    {
        // Arrange
        var invoiceId = 1;
        _invoiceRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(new Invoice { Id = 1, IdOrder = 2, InvoiceNumber = "xxxxx", Iva = 12, Status = true, SubTotal = 200, Total = 210, SystemDate = new DateTime()});

        // Act
        var result = await _invoiceService.GetById(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceId, result.Id);
    }

    [Fact]
    public async Task CreateInvoice_CreatesNewInvoice()
    {
        // Arrange
        var invoicePayload = new Invoice
        {
            Id = 2,
            IdOrder = 2,
            InvoiceNumber = "xxxxx",
            Iva = 12,
            Status = true,
            SubTotal = 200,
            Total = 210,
            SystemDate = new DateTime()
        };
        _invoiceRepository.Setup(x => x.Create(It.IsAny<Invoice>()));
        _invoiceRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(invoicePayload);

        // Act
        await _invoiceService.Create(invoicePayload);
        var result = await _invoiceService.GetById(invoicePayload.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoicePayload.Id, result.Id);
    }

    [Fact]
    public async Task DeleteInvoices_By_Id_return_True()
    {
        // Arrange
        var invoiceId = 1;
        _invoiceRepository.Setup(x => x.Delete(It.IsAny<int>()));
        _invoiceRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((Invoice)null);

        // Act
        await _invoiceService.Delete(invoiceId);
        var result = await _invoiceService.GetById(invoiceId);

        // Assert
        Assert.Null(result);
    }
}