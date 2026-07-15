namespace TicketSystem.Api.Models.Enums;

public enum TicketStatus
{
    Open,
    InProgress,
    Resolved,
    Closed,
    Cancelled
}

public static class TicketStatusExtensions
{
    private static readonly HashSet<string> ValidValues =
    [
        nameof(TicketStatus.Open),
        nameof(TicketStatus.InProgress),
        nameof(TicketStatus.Resolved),
        nameof(TicketStatus.Closed),
        nameof(TicketStatus.Cancelled)
    ];

    public static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && ValidValues.Contains(value);

    public static TicketStatus Parse(string value) =>
        Enum.Parse<TicketStatus>(value, ignoreCase: false);

    public static string ToStatusString(this TicketStatus status) => status.ToString();
}
