using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Blazor.Models;

public class CreateCommentRequest
{
    [Required(ErrorMessage = "Comment is required.")]
    [MaxLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters.")]
    public string Message { get; set; } = string.Empty;
}
