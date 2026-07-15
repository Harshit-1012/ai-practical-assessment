namespace TicketSystem.Api.DTOs;

public class ApiError
{
    public string? Error { get; set; }
    public string? Detail { get; set; }
    public string? RequestId { get; set; }
    public string? CurrentStatus { get; set; }
    public string? RequestedStatus { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
