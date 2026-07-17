using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Api.DTOs;

public class LoginDto
{
    [Required]
    public int UserId { get; set; }
}
