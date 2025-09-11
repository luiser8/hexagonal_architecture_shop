namespace HexagonalShop.Application.DTOs;

public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Token { get; set; }
    public bool? Status { get; set; } = true;
    public DateTime? SystemDate { get; set; } = new DateTime();
}