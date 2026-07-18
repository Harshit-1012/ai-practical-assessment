using System.ComponentModel.DataAnnotations;
using TicketSystem.Api.Models.Enums;

namespace TicketSystem.Tests.UnitTests;

public class TicketEnumValidationTests
{
    [Theory]
    [InlineData("Open")]
    [InlineData("InProgress")]
    [InlineData("Resolved")]
    [InlineData("Closed")]
    [InlineData("Cancelled")]
    public void TicketStatus_IsValid_KnownValues_ReturnsTrue(string status) =>
        Assert.True(TicketStatusExtensions.IsValid(status));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("NotARealStatus")]
    [InlineData("open")]
    public void TicketStatus_IsValid_InvalidValues_ReturnsFalse(string? status) =>
        Assert.False(TicketStatusExtensions.IsValid(status));

    [Theory]
    [InlineData("Low")]
    [InlineData("Medium")]
    [InlineData("High")]
    [InlineData("Critical")]
    public void TicketPriority_IsValid_KnownValues_ReturnsTrue(string priority) =>
        Assert.True(TicketPriorityExtensions.IsValid(priority));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Urgent")]
    [InlineData("low")]
    public void TicketPriority_IsValid_InvalidValues_ReturnsFalse(string? priority) =>
        Assert.False(TicketPriorityExtensions.IsValid(priority));
}
