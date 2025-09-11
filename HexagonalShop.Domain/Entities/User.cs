using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HexagonalShop.Domain.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(155)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(155)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(155)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;
    
    [MaxLength(255)]
    [Column("token")]
    public string Token { get; set; } = string.Empty;

    [Column("systemDate")]
    public DateTime SystemDate { get; set; } = DateTime.Now;

    [Column("status")]
    public bool Status { get; set; } = true;
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
