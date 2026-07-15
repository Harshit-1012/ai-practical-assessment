using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Blazor.Models;

public class UpdateTicketRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(5000, ErrorMessage = "Description cannot exceed 5000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Priority is required.")]
    public string Priority { get; set; } = "Medium";

    public int? AssignedToId { get; set; }
}
