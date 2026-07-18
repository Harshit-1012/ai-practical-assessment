namespace TicketSystem.Api.DTOs;

public class TicketListQueryDto
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;

    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public string? Priority { get; set; }
    public int? AssignedToId { get; set; }
    public bool UnassignedOnly { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int PageNumber { get; set; } = DefaultPageNumber;
    public int PageSize { get; set; } = DefaultPageSize;
}
