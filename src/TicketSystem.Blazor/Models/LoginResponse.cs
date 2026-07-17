namespace TicketSystem.Blazor.Models;

public class LoginResponse
{
    public required string Token { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}
