using System.ComponentModel.DataAnnotations;
using TicketSystem.Api.Validation;

namespace TicketSystem.Api.DTOs;

public class CreateCommentDto
{
    [Required]
    [NotWhitespace]
    [MaxLength(2000)]
    public required string Message { get; set; }
}
