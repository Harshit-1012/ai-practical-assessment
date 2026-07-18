using System.ComponentModel.DataAnnotations;
using TicketSystem.Api.Validation;

namespace TicketSystem.Api.DTOs;

public class UpdateTicketDto
{
    [Required]
    [NotWhitespace]
    [MaxLength(200)]
    public required string Title { get; set; }

    [Required]
    [NotWhitespace]
    [MaxLength(5000)]
    public required string Description { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Priority { get; set; }

    public int? AssignedToId { get; set; }
}
