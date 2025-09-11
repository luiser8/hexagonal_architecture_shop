using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HexagonalShop.Domain.Entities;

[Table("order_details")]
public class OrderDetails
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("idOrder")]
    public int IdOrder { get; set; }

    [Required]
    [Column("idProduct")]
    public int IdProduct { get; set; }

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("systemDate")]
    public DateTime SystemDate { get; set; } = DateTime.Now;

    [ForeignKey(nameof(IdOrder))]
    public Order? Order { get; set; }
    
    [ForeignKey(nameof(IdProduct))]
    public Product? Product { get; set; }
}