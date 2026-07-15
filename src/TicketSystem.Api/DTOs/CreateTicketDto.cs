using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Api.DTOs;

public class CreateTicketDto
{
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }

    [Required]
    [MaxLength(5000)]
    public required string Description { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Priority { get; set; }

    public int? AssignedToId { get; set; }

    [Required]
    public int CreatedById { get; set; }
}
