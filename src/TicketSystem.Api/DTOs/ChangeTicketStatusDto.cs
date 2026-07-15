using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Api.DTOs;

public class ChangeTicketStatusDto
{
    [Required]
    [MaxLength(50)]
    public required string NewStatus { get; set; }
}
