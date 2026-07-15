using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Api.DTOs;

public class CreateCommentDto
{
    [Required]
    [MaxLength(2000)]
    public required string Message { get; set; }

    [Required]
    public int CreatedById { get; set; }
}
