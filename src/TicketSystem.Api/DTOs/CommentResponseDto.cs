namespace TicketSystem.Api.DTOs;

public class CommentResponseDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public required string Message { get; set; }
    public int CreatedById { get; set; }
    public required string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}
