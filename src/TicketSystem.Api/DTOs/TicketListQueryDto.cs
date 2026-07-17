namespace TicketSystem.Api.DTOs;

public class TicketListQueryDto
{
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public string? Priority { get; set; }
    public int? AssignedToId { get; set; }
    public bool UnassignedOnly { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
