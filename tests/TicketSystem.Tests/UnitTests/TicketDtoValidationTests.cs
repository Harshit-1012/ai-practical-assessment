using System.ComponentModel.DataAnnotations;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Tests.UnitTests;

public class TicketDtoValidationTests
{
    [Fact]
    public void CreateTicketDto_MissingTitle_HasValidationError()
    {
        var dto = ValidCreateTicketDto();
        dto.Title = "";

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateTicketDto.Title)));
    }

    [Fact]
    public void CreateTicketDto_MissingDescription_HasValidationError()
    {
        var dto = ValidCreateTicketDto();
        dto.Description = "";

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateTicketDto.Description)));
    }

    [Fact]
    public void CreateTicketDto_WhitespaceOnlyTitle_HasValidationError()
    {
        var dto = ValidCreateTicketDto();
        dto.Title = "   ";

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateTicketDto.Title)));
    }

    [Fact]
    public void CreateTicketDto_WhitespaceOnlyDescription_HasValidationError()
    {
        var dto = ValidCreateTicketDto();
        dto.Description = "\t";

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateTicketDto.Description)));
    }

    [Fact]
    public void CreateTicketDto_TitleExceedsMaxLength_HasValidationError()
    {
        var dto = ValidCreateTicketDto();
        dto.Title = new string('a', 201);

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateTicketDto.Title)));
    }

    [Fact]
    public void CreateTicketDto_ValidData_HasNoValidationErrors()
    {
        var results = Validate(ValidCreateTicketDto());

        Assert.Empty(results);
    }

    [Fact]
    public void UpdateTicketDto_MissingTitle_HasValidationError()
    {
        var dto = ValidUpdateTicketDto();
        dto.Title = "";

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UpdateTicketDto.Title)));
    }

    [Fact]
    public void CreateCommentDto_MissingMessage_HasValidationError()
    {
        var dto = new CreateCommentDto { Message = "" };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateCommentDto.Message)));
    }

    [Fact]
    public void CreateCommentDto_WhitespaceOnlyMessage_HasValidationError()
    {
        var dto = new CreateCommentDto { Message = "   " };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateCommentDto.Message)));
    }

    [Fact]
    public void CreateCommentDto_MessageExceedsMaxLength_HasValidationError()
    {
        var dto = new CreateCommentDto { Message = new string('a', 2001) };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateCommentDto.Message)));
    }

    private static CreateTicketDto ValidCreateTicketDto() =>
        new()
        {
            Title = "Valid title",
            Description = "Valid description",
            Priority = "Medium"
        };

    private static UpdateTicketDto ValidUpdateTicketDto() =>
        new()
        {
            Title = "Valid title",
            Description = "Valid description",
            Priority = "Medium"
        };

    private static List<ValidationResult> Validate(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
