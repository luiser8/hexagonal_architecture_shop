using HexagonalShop.Domain.Entities;

namespace HexagonalShop.Application.DTOs;

public class OrderDto
{
    public int IdUser { get; set; }
    public string? IdempotencyKey { get; set; } = Guid.Empty.ToString();
    public bool? Status { get; set; } = true;
    public DateTime? SystemDate { get; set; } = new DateTime();
    public List<OrderDetailsDto>  OrderDetails { get; set; } = [];
}