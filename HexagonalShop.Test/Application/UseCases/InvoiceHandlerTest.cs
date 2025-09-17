using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using Moq;
using Xunit;

namespace HexagonalShop.Test.Application.UseCases;

public class InvoiceHandlerTest
{
  private readonly Mock<IInvoiceService> _invoiceServiceMock;
  private readonly InvoiceHandler _invoiceHandler;

  public InvoiceHandlerTest()
  {
    _invoiceServiceMock = new Mock<IInvoiceService>();
    _invoiceHandler = new InvoiceHandler(_invoiceServiceMock.Object);
  }

  [Fact]
  public async Task ExecuteById_ExistingId_ReturnsInvoice()
  {
    // Arrange
    var invoiceId = 1;
    var expectedInvoice = new Invoice { Id = invoiceId };
    _invoiceServiceMock.Setup(x => x.GetById(invoiceId)).ReturnsAsync(expectedInvoice);

    // Act
    var result = await _invoiceHandler.ExecuteById(invoiceId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedInvoice.Id, result.Id);
  }

  [Fact]
  public async Task ExecuteDelete_ExistingId_ReturnTrue()
  {
    // Arrange
    var invoiceId = 1;
    _invoiceServiceMock.Setup(x => x.Delete(invoiceId));
    _invoiceServiceMock.Setup(x => x.GetById(invoiceId)).ReturnsAsync((Invoice)null);

    // Act
    await _invoiceHandler.ExecuteDelete(invoiceId);
    var result = await _invoiceHandler.ExecuteById(invoiceId);

    // Assert
    Assert.Null(result);
  }
}