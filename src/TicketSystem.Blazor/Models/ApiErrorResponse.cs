namespace TicketSystem.Blazor.Models;

public class ApiErrorResponse
{
    public string? Error { get; set; }
    public string? Detail { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
