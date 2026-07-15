namespace TicketSystem.Api.Models.Enums;

public enum TicketPriority
{
    Low,
    Medium,
    High,
    Critical
}

public static class TicketPriorityExtensions
{
    private static readonly HashSet<string> ValidValues =
    [
        nameof(TicketPriority.Low),
        nameof(TicketPriority.Medium),
        nameof(TicketPriority.High),
        nameof(TicketPriority.Critical)
    ];

    public static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && ValidValues.Contains(value);
}
