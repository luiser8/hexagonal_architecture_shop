using HexagonalShop.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HexagonalShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly InvoiceHandler _invoiceHandler;
    public InvoiceController(InvoiceHandler invoiceHandler) => _invoiceHandler = invoiceHandler;
    
    [HttpGet, Authorize]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _invoiceHandler.ExecuteById(id));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _invoiceHandler.ExecuteDelete(id);
        return Ok("Invoice deleted successfully");
    }
}