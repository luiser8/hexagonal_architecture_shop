namespace HexagonalShop.Application.DTOs;

public class InvoiceDto
{
    public int IdOrder { get; set; }
    public int IdUser { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public bool? Status { get; set; } = true;
    public DateTime? SystemDate { get; set; } = new DateTime();
}