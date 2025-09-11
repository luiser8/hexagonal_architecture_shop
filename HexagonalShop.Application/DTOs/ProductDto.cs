namespace HexagonalShop.Application.DTOs;

public class ProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public bool Status { get; set; } = true;
    public DateTime? SystemDate { get; set; } = new DateTime();
}