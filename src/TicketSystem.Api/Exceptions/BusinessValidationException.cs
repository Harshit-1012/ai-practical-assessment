namespace TicketSystem.Api.Exceptions;

public class BusinessValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public BusinessValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public BusinessValidationException(string field, string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }
}
