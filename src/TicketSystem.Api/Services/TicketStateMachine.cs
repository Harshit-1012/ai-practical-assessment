using TicketSystem.Api.Exceptions;
using TicketSystem.Api.Models.Enums;

namespace TicketSystem.Api.Services;

public class TicketStateMachine : ITicketStateMachine
{
    private static readonly Dictionary<TicketStatus, TicketStatus[]> AllowedTransitions = new()
    {
        [TicketStatus.Open] = [TicketStatus.InProgress, TicketStatus.Cancelled],
        [TicketStatus.InProgress] = [TicketStatus.Resolved, TicketStatus.Cancelled],
        [TicketStatus.Resolved] = [TicketStatus.Closed],
        [TicketStatus.Closed] = [],
        [TicketStatus.Cancelled] = []
    };

    public bool CanTransition(TicketStatus from, TicketStatus to) =>
        AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public void ValidateTransition(TicketStatus from, TicketStatus to)
    {
        if (CanTransition(from, to))
        {
            return;
        }

        var validTargets = AllowedTransitions.TryGetValue(from, out var allowed)
            ? string.Join(", ", allowed.Select(s => s.ToString()))
            : "none";

        var message = validTargets == "none"
            ? $"Cannot transition from {from} to {to}. No further transitions are allowed."
            : $"Cannot transition from {from} to {to}. Valid transitions: {validTargets}";

        throw new InvalidTransitionException(from.ToString(), to.ToString(), message);
    }
}
