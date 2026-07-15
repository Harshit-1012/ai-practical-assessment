using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

/// <summary>
/// Mirrors backend state machine rules for UI presentation only.
/// Authoritative validation remains on the API.
/// </summary>
public interface ITicketWorkflowService
{
    IReadOnlyList<StatusTransitionAction> GetTransitionActions(string currentStatus);
    bool CanTransition(string fromStatus, string toStatus);
    string GetInvalidTransitionReason(string fromStatus, string toStatus);
}

public class TicketWorkflowService : ITicketWorkflowService
{
    private static readonly Dictionary<string, string[]> AllowedTransitions = new(StringComparer.Ordinal)
    {
        ["Open"] = ["InProgress", "Cancelled"],
        ["InProgress"] = ["Resolved", "Cancelled"],
        ["Resolved"] = ["Closed"],
        ["Closed"] = [],
        ["Cancelled"] = []
    };

    private static readonly string[] AllStatuses = ["Open", "InProgress", "Resolved", "Closed", "Cancelled"];

    private static readonly Dictionary<string, string> ActionLabels = new(StringComparer.Ordinal)
    {
        ["InProgress"] = "Start Progress",
        ["Resolved"] = "Mark Resolved",
        ["Closed"] = "Close Ticket",
        ["Cancelled"] = "Cancel Ticket"
    };

    public IReadOnlyList<StatusTransitionAction> GetTransitionActions(string currentStatus)
    {
        return AllStatuses
            .Where(status => !string.Equals(status, currentStatus, StringComparison.Ordinal))
            .Select(target => CreateAction(currentStatus, target))
            .ToList();
    }

    public bool CanTransition(string fromStatus, string toStatus) =>
        AllowedTransitions.TryGetValue(fromStatus, out var allowed) && allowed.Contains(toStatus);

    public string GetInvalidTransitionReason(string fromStatus, string toStatus)
    {
        if (string.Equals(fromStatus, toStatus, StringComparison.Ordinal))
        {
            return "Ticket is already in this status.";
        }

        if (!AllowedTransitions.TryGetValue(fromStatus, out var allowed) || allowed.Length == 0)
        {
            return $"Tickets in {fromStatus} status cannot be changed further.";
        }

        var validTargets = string.Join(", ", allowed);
        return $"Cannot move from {fromStatus} to {toStatus}. Allowed: {validTargets}.";
    }

    private StatusTransitionAction CreateAction(string currentStatus, string targetStatus)
    {
        var isEnabled = CanTransition(currentStatus, targetStatus);
        return new StatusTransitionAction
        {
            TargetStatus = targetStatus,
            Label = ActionLabels.GetValueOrDefault(targetStatus, targetStatus),
            IsEnabled = isEnabled,
            DisabledReason = isEnabled ? null : GetInvalidTransitionReason(currentStatus, targetStatus),
            ButtonClass = isEnabled ? "btn-transition btn-transition--enabled" : "btn-transition btn-transition--disabled"
        };
    }
}
