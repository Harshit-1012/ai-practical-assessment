using TicketSystem.Api.Exceptions;

namespace TicketSystem.Api.Validation;

public static class InputValidation
{
    public static string RequireTrimmedNonWhitespace(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessValidationException(fieldName, $"{fieldName} cannot be empty or whitespace.");
        }

        return value.Trim();
    }
}
