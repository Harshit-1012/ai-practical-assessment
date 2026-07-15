namespace TicketSystem.Api.Exceptions;

public class InvalidTransitionException : Exception
{
    public string CurrentStatus { get; }
    public string RequestedStatus { get; }

    public InvalidTransitionException(string currentStatus, string requestedStatus, string message)
        : base(message)
    {
        CurrentStatus = currentStatus;
        RequestedStatus = requestedStatus;
    }
}
