namespace TicketSystem.Blazor.Models;

public class StatusTransitionAction
{
    public required string TargetStatus { get; init; }
    public required string Label { get; init; }
    public bool IsEnabled { get; init; }
    public string? DisabledReason { get; init; }
    public required string ButtonClass { get; init; }
}
