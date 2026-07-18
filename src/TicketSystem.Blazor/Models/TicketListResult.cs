namespace TicketSystem.Blazor.Models;

public class TicketListResult
{
    public IReadOnlyList<TicketDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
