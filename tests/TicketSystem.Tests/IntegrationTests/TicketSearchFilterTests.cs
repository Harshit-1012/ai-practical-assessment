using System.Net;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Models.Enums;
using TicketSystem.Tests.Helpers;

namespace TicketSystem.Tests.IntegrationTests;

public class TicketSearchFilterTests : IntegrationTestBase
{
    public TicketSearchFilterTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetTickets_FilterByStatus_ReturnsMatchingTicketsOnly()
    {
        await SeedTicketAsync("Open ticket", TicketStatus.Open);
        await SeedTicketAsync("In progress ticket", TicketStatus.InProgress);
        await SeedTicketAsync("Another open ticket", TicketStatus.Open);

        var (statusCode, body) = await Client.GetJsonAsync<TicketListResponseDto>(
            $"/api/tickets?status={TicketStatus.Open}");

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Equal(2, body.Items.Count);
        Assert.Equal(2, body.TotalCount);
        Assert.All(body.Items, t => Assert.Equal(TicketStatus.Open.ToString(), t.Status));
    }

    [Fact]
    public async Task GetTickets_SearchByKeyword_ReturnsMatchingTickets()
    {
        await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(
                title: "Database connection failure",
                description: "Cannot connect to reporting database"));

        await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(
                title: "Laptop request",
                description: "Need a new laptop for onboarding"));

        var (statusCode, body) = await Client.GetJsonAsync<TicketListResponseDto>(
            "/api/tickets?keyword=database");

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Single(body.Items);
        Assert.Equal(1, body.TotalCount);
        Assert.Contains("database", body.Items[0].Title, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetTickets_FilterByStatusAndKeyword_ReturnsIntersection()
    {
        await SeedTicketAsync("Open database issue", TicketStatus.Open, "Database timeout in production");
        await SeedTicketAsync("Closed database issue", TicketStatus.Closed, "Old database incident");
        await SeedTicketAsync("Open printer issue", TicketStatus.Open, "Printer jam on floor 3");

        var (statusCode, body) = await Client.GetJsonAsync<TicketListResponseDto>(
            $"/api/tickets?status={TicketStatus.Open}&keyword=database");

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Single(body.Items);
        Assert.Equal(1, body.TotalCount);
        Assert.Equal("Open database issue", body.Items[0].Title);
        Assert.Equal(TicketStatus.Open.ToString(), body.Items[0].Status);
    }

    [Fact]
    public async Task GetTickets_InvalidStatusFilter_Returns400BadRequest()
    {
        var response = await Client.GetAsync("/api/tickets?status=NotARealStatus");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.ReadApiErrorAsync();
        Assert.NotNull(error);
        Assert.NotNull(error.Errors);
        Assert.True(error.Errors.ContainsKey("status"));
    }

    private async Task SeedTicketAsync(string title, TicketStatus status, string description = "Seeded ticket")
    {
        await TestDataSeeder.SeedTicketAsync(
            Factory.Services,
            status.ToString(),
            title: title,
            description: description);
    }
}
