namespace TicketSystem.Blazor.Services;

public interface ITicketDisplayService
{
    string FormatDateTime(DateTime value);
    string FormatRelativeDate(DateTime value);
    string GetStatusLabel(string status);
    string GetStatusBadgeClass(string status);
    string GetPriorityLabel(string priority);
    string GetPriorityBadgeClass(string priority);
    IReadOnlyList<string> GetPriorityOptions();
    IReadOnlyList<(string Value, string Label)> GetStatusFilterOptions();
    string GetAssigneeDisplay(string? assigneeName);
}

public class TicketDisplayService : ITicketDisplayService
{
    private static readonly Dictionary<string, string> StatusLabels = new(StringComparer.Ordinal)
    {
        ["Open"] = "Open",
        ["InProgress"] = "In Progress",
        ["Resolved"] = "Resolved",
        ["Closed"] = "Closed",
        ["Cancelled"] = "Cancelled"
    };

    private static readonly Dictionary<string, string> StatusClasses = new(StringComparer.Ordinal)
    {
        ["Open"] = "badge-status badge-status--open",
        ["InProgress"] = "badge-status badge-status--inprogress",
        ["Resolved"] = "badge-status badge-status--resolved",
        ["Closed"] = "badge-status badge-status--closed",
        ["Cancelled"] = "badge-status badge-status--cancelled"
    };

    private static readonly Dictionary<string, string> PriorityClasses = new(StringComparer.Ordinal)
    {
        ["Low"] = "badge-priority badge-priority--low",
        ["Medium"] = "badge-priority badge-priority--medium",
        ["High"] = "badge-priority badge-priority--high",
        ["Critical"] = "badge-priority badge-priority--critical"
    };

    public string FormatDateTime(DateTime value) =>
        value.ToLocalTime().ToString("MMM d, yyyy h:mm tt");

    public string FormatRelativeDate(DateTime value)
    {
        var local = value.ToLocalTime();
        var diff = DateTime.Now - local;

        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalDays < 1) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
        return local.ToString("MMM d, yyyy");
    }

    public string GetStatusLabel(string status) =>
        StatusLabels.GetValueOrDefault(status, status);

    public string GetStatusBadgeClass(string status) =>
        StatusClasses.GetValueOrDefault(status, "badge-status");

    public string GetPriorityLabel(string priority) => priority;

    public string GetPriorityBadgeClass(string priority) =>
        PriorityClasses.GetValueOrDefault(priority, "badge-priority");

    public IReadOnlyList<string> GetPriorityOptions() =>
        ["Low", "Medium", "High", "Critical"];

    public IReadOnlyList<(string Value, string Label)> GetStatusFilterOptions() =>
    [
        ("All", "All Statuses"),
        ("Open", "Open"),
        ("InProgress", "In Progress"),
        ("Resolved", "Resolved"),
        ("Closed", "Closed"),
        ("Cancelled", "Cancelled")
    ];

    public string GetAssigneeDisplay(string? assigneeName) =>
        string.IsNullOrWhiteSpace(assigneeName) ? "Unassigned" : assigneeName;
}
