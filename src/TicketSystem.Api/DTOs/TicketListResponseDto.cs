namespace TicketSystem.Api.DTOs;

public class TicketListResponseDto
{
    public required IReadOnlyList<TicketResponseDto> Items { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
