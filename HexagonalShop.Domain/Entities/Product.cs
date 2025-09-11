using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HexagonalShop.Domain.Entities;


[Table("products")]
public class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(155)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("stock")]
    public int Stock { get; set; }
    
    [Column("price")]
    public decimal Price { get; set; }

    [Column("systemDate")]
    public DateTime SystemDate { get; set; } = DateTime.Now;

    [Column("status")]
    public bool Status { get; set; } = true;
    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}
