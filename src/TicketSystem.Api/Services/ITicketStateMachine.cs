using TicketSystem.Api.Models.Enums;

namespace TicketSystem.Api.Services;

public interface ITicketStateMachine
{
    bool CanTransition(TicketStatus from, TicketStatus to);
    void ValidateTransition(TicketStatus from, TicketStatus to);
}
