using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Api.Validation;

public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    public NotWhitespaceAttribute()
    {
        ErrorMessage = "{0} cannot be empty or whitespace.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                [validationContext.MemberName!]);
        }

        return ValidationResult.Success;
    }
}
