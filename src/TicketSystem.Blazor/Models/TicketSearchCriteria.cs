namespace TicketSystem.Blazor.Models;

public class TicketSearchCriteria
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;

    public string? Keyword { get; init; }
    public string? Status { get; init; }
    public string? Priority { get; init; }
    public int? AssignedToId { get; init; }
    public bool UnassignedOnly { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
    public int PageNumber { get; init; } = DefaultPageNumber;
    public int PageSize { get; init; } = DefaultPageSize;
}
