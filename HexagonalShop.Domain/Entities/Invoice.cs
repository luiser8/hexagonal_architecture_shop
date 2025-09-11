using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace HexagonalShop.Domain.Entities;

[Table("invoices")]
public class Invoice
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("invoiceNumber")]
    public string InvoiceNumber { get; set; } = NumberInvoice(8);
    
    [Required]
    [Column("idOrder")]
    public int IdOrder { get; set; }
    
    [Column("subTotal")]
    public decimal SubTotal { get; set; }
    
    [Column("total")]
    public decimal Total { get; set; }
    
    [Column("iva")]
    public decimal Iva { get; set; }

    [Column("status")]
    public bool Status { get; set; } = true;

    [Column("systemDate")]
    public DateTime SystemDate { get; set; } = DateTime.Now;
    
    [ForeignKey(nameof(IdOrder))]
    public Order? Order { get; set; }
    
    private static string NumberInvoice(int longitud)
    {
        var chars = new char[longitud];
        using var rng = RandomNumberGenerator.Create();

        byte[] buffer = new byte[1];
        for (int i = 0; i < longitud; i++)
        {
            rng.GetBytes(buffer);
            chars[i] = (char)('0' + (buffer[0] % 10)); // dígito 0–9
        }

        return new string(chars);
    }
}