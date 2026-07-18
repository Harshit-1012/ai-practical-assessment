using TicketSystem.Api.Exceptions;
using TicketSystem.Api.Models.Enums;
using TicketSystem.Api.Services;

namespace TicketSystem.Tests.UnitTests;

public class TicketStateMachineTests
{
    private readonly TicketStateMachine _stateMachine = new();

    public static TheoryData<TicketStatus, TicketStatus> ValidTransitionPairs => new()
    {
        { TicketStatus.Open, TicketStatus.InProgress },
        { TicketStatus.Open, TicketStatus.Cancelled },
        { TicketStatus.InProgress, TicketStatus.Resolved },
        { TicketStatus.InProgress, TicketStatus.Cancelled },
        { TicketStatus.Resolved, TicketStatus.Closed }
    };

    public static IEnumerable<object[]> InvalidTransitionPairs
    {
        get
        {
            var machine = new TicketStateMachine();
            foreach (var from in Enum.GetValues<TicketStatus>())
            {
                foreach (var to in Enum.GetValues<TicketStatus>())
                {
                    if (!machine.CanTransition(from, to))
                    {
                        yield return [from, to];
                    }
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(ValidTransitionPairs))]
    public void CanTransition_ValidPair_ReturnsTrue(TicketStatus from, TicketStatus to) =>
        Assert.True(_stateMachine.CanTransition(from, to));

    [Theory]
    [MemberData(nameof(InvalidTransitionPairs))]
    public void CanTransition_InvalidPair_ReturnsFalse(TicketStatus from, TicketStatus to) =>
        Assert.False(_stateMachine.CanTransition(from, to));

    [Theory]
    [MemberData(nameof(ValidTransitionPairs))]
    public void ValidateTransition_ValidPair_DoesNotThrow(TicketStatus from, TicketStatus to)
    {
        var exception = Record.Exception(() => _stateMachine.ValidateTransition(from, to));
        Assert.Null(exception);
    }

    [Theory]
    [MemberData(nameof(InvalidTransitionPairs))]
    public void ValidateTransition_InvalidPair_ThrowsInvalidTransitionException(TicketStatus from, TicketStatus to)
    {
        var exception = Assert.Throws<InvalidTransitionException>(
            () => _stateMachine.ValidateTransition(from, to));

        Assert.Equal(from.ToString(), exception.CurrentStatus);
        Assert.Equal(to.ToString(), exception.RequestedStatus);
        Assert.False(string.IsNullOrWhiteSpace(exception.Message));
    }

    [Fact]
    public void CanTransition_SameStatus_ReturnsFalse() =>
        Assert.False(_stateMachine.CanTransition(TicketStatus.Open, TicketStatus.Open));

    [Fact]
    public void ValidateTransition_FromClosed_ThrowsWithTerminalMessage()
    {
        var exception = Assert.Throws<InvalidTransitionException>(
            () => _stateMachine.ValidateTransition(TicketStatus.Closed, TicketStatus.Open));

        Assert.Contains("Cannot transition from Closed to Open", exception.Message);
    }

    [Fact]
    public void ValidateTransition_FromOpenToResolved_ThrowsWithValidTargetsMessage()
    {
        var exception = Assert.Throws<InvalidTransitionException>(
            () => _stateMachine.ValidateTransition(TicketStatus.Open, TicketStatus.Resolved));

        Assert.Contains("Valid transitions", exception.Message);
        Assert.Contains("InProgress", exception.Message);
        Assert.Contains("Cancelled", exception.Message);
    }
}
