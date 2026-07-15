namespace TicketSystem.Api.Models;

public class Ticket
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Priority { get; set; } // Low, Medium, High, Critical
    public required string Status { get; set; } // Open, InProgress, Resolved, Closed, Cancelled
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Comment> Comments { get; set; } = new();
}
