using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HexagonalShop.Domain.Entities;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("idempotency_key")]
    public string? IdempotencyKey { get; set; } = Guid.Empty.ToString();

    [Required]
    [Column("idUser")]
    public int IdUser { get; set; }

    [Column("status")]
    public bool Status { get; set; } = true;

    [Column("systemDate")]
    public DateTime SystemDate { get; set; } = DateTime.Now;

    [ForeignKey("IdUser")]
    public User? User { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}