using HexagonalShop.Application.UseCases;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;
using HexagonalShop.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Controllers;

public class InvoiceControllerTest
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly InvoiceHandler _invoiceHandler;
    private readonly InvoiceController _invoiceController;

    public InvoiceControllerTest()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _invoiceHandler = new InvoiceHandler(_invoiceServiceMock.Object);
        _invoiceController = new InvoiceController(_invoiceHandler);
    }

    [Fact]
    public async Task GetInvoice_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var invoiceId = 1;
        var expectedInvoice = new Invoice { Id = invoiceId, SubTotal = 100, Total = 200 };
        _invoiceServiceMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(expectedInvoice);

        // Act
        var result = await _invoiceController.Get(invoiceId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedInvoice, okResult.Value);
    }

    [Fact]
    public async Task DeleteInvoice_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var invoiceId = 1;
        _invoiceServiceMock.Setup(x => x.Delete(It.IsAny<int>()));

        // Act
        var result = await _invoiceController.Delete(invoiceId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Invoice deleted successfully", okResult.Value);
    }
}