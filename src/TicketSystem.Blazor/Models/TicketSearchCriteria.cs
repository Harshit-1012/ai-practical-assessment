namespace TicketSystem.Blazor.Models;

public class TicketSearchCriteria
{
    public string? Keyword { get; init; }
    public string? Status { get; init; }
    public string? Priority { get; init; }
    public int? AssignedToId { get; init; }
    public bool UnassignedOnly { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}
