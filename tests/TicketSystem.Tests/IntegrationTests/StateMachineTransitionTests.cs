using System.Net;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Models.Enums;
using TicketSystem.Tests.Helpers;

namespace TicketSystem.Tests.IntegrationTests;

public class StateMachineTransitionTests : IntegrationTestBase
{
    public StateMachineTransitionTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    public static TheoryData<string, string> ValidTransitions => new()
    {
        { TicketStatus.Open.ToString(), TicketStatus.InProgress.ToString() },
        { TicketStatus.Open.ToString(), TicketStatus.Cancelled.ToString() },
        { TicketStatus.InProgress.ToString(), TicketStatus.Resolved.ToString() },
        { TicketStatus.InProgress.ToString(), TicketStatus.Cancelled.ToString() },
        { TicketStatus.Resolved.ToString(), TicketStatus.Closed.ToString() }
    };

    public static TheoryData<string, string> InvalidTransitions => new()
    {
        { TicketStatus.Open.ToString(), TicketStatus.Resolved.ToString() },
        { TicketStatus.Open.ToString(), TicketStatus.Closed.ToString() },
        { TicketStatus.Resolved.ToString(), TicketStatus.InProgress.ToString() },
        { TicketStatus.Closed.ToString(), TicketStatus.Open.ToString() },
        { TicketStatus.Cancelled.ToString(), TicketStatus.InProgress.ToString() },
        { TicketStatus.Closed.ToString(), TicketStatus.InProgress.ToString() },
        { TicketStatus.Closed.ToString(), TicketStatus.Resolved.ToString() },
        { TicketStatus.Closed.ToString(), TicketStatus.Cancelled.ToString() },
        { TicketStatus.Cancelled.ToString(), TicketStatus.Open.ToString() },
        { TicketStatus.Cancelled.ToString(), TicketStatus.Resolved.ToString() },
        { TicketStatus.Cancelled.ToString(), TicketStatus.Closed.ToString() }
    };

    [Theory]
    [MemberData(nameof(ValidTransitions))]
    public async Task ChangeStatus_ValidTransition_Returns200AndPersistsStatus(string fromStatus, string toStatus)
    {
        var ticketId = await PrepareTicketInStatusAsync(fromStatus);

        var (statusCode, body) = await Client.PutJsonAsync<TicketResponseDto>(
            $"/api/tickets/{ticketId}/status",
            new ChangeTicketStatusDto { NewStatus = toStatus });

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Equal(toStatus, body.Status);

        var persistedStatus = await TestDataSeeder.GetTicketStatusAsync(Factory.Services, ticketId);
        Assert.Equal(toStatus, persistedStatus);

        var (getStatusCode, getBody) = await Client.GetJsonAsync<TicketResponseDto>($"/api/tickets/{ticketId}");
        Assert.Equal(HttpStatusCode.OK, getStatusCode);
        Assert.NotNull(getBody);
        Assert.Equal(toStatus, getBody.Status);
    }

    [Theory]
    [MemberData(nameof(InvalidTransitions))]
    public async Task ChangeStatus_InvalidTransition_Returns400BadRequest(string fromStatus, string toStatus)
    {
        var ticketId = await PrepareTicketInStatusAsync(fromStatus);

        var response = await Client.PutAsJsonAsync(
            $"/api/tickets/{ticketId}/status",
            new ChangeTicketStatusDto { NewStatus = toStatus });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.ReadApiErrorAsync();
        Assert.NotNull(error);
        Assert.Equal("Invalid status transition", error.Error);
        Assert.Equal(fromStatus, error.CurrentStatus);
        Assert.Equal(toStatus, error.RequestedStatus);

        var persistedStatus = await TestDataSeeder.GetTicketStatusAsync(Factory.Services, ticketId);
        Assert.Equal(fromStatus, persistedStatus);
    }

    private async Task<int> PrepareTicketInStatusAsync(string targetStatus)
    {
        return targetStatus switch
        {
            nameof(TicketStatus.Open) => await CreateTicketViaApiAsync(),
            nameof(TicketStatus.InProgress) => await TransitionToAsync(
                await CreateTicketViaApiAsync(),
                TicketStatus.InProgress.ToString()),
            nameof(TicketStatus.Resolved) => await TransitionToAsync(
                await TransitionToAsync(await CreateTicketViaApiAsync(), TicketStatus.InProgress.ToString()),
                TicketStatus.Resolved.ToString()),
            nameof(TicketStatus.Closed) => await TestDataSeeder.SeedTicketAsync(
                Factory.Services,
                TicketStatus.Closed.ToString(),
                title: $"Closed ticket {Guid.NewGuid():N}"),
            nameof(TicketStatus.Cancelled) => await TestDataSeeder.SeedTicketAsync(
                Factory.Services,
                TicketStatus.Cancelled.ToString(),
                title: $"Cancelled ticket {Guid.NewGuid():N}"),
            _ => throw new ArgumentOutOfRangeException(nameof(targetStatus), targetStatus, "Unsupported ticket status.")
        };
    }

    private async Task<int> CreateTicketViaApiAsync()
    {
        var (statusCode, body) = await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(title: $"Ticket {Guid.NewGuid():N}"));

        Assert.Equal(HttpStatusCode.Created, statusCode);
        Assert.NotNull(body);
        return body.Id;
    }

    private async Task<int> TransitionToAsync(int ticketId, string newStatus)
    {
        var (statusCode, _) = await Client.PutJsonAsync<TicketResponseDto>(
            $"/api/tickets/{ticketId}/status",
            new ChangeTicketStatusDto { NewStatus = newStatus });

        Assert.Equal(HttpStatusCode.OK, statusCode);
        return ticketId;
    }
}
