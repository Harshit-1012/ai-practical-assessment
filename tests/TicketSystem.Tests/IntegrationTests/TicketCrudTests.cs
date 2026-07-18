using System.Net;
using TicketSystem.Api.DTOs;
using TicketSystem.Tests.Helpers;

namespace TicketSystem.Tests.IntegrationTests;

public class TicketCrudTests : IntegrationTestBase
{
    public TicketCrudTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateTicket_ValidData_Returns201Created()
    {
        var dto = TestDataSeeder.CreateValidTicketDto(
            title: "Printer not working",
            description: "Office printer on floor 2 is offline");

        var (statusCode, body) = await Client.PostJsonAsync<TicketResponseDto>("/api/tickets", dto);

        Assert.Equal(HttpStatusCode.Created, statusCode);
        Assert.NotNull(body);
        Assert.True(body.Id > 0);
        Assert.Equal(dto.Title, body.Title);
        Assert.Equal(dto.Description, body.Description);
        Assert.Equal(dto.Priority, body.Priority);
        Assert.Equal("Open", body.Status);
        Assert.Equal(TestDataSeeder.DefaultAuthenticatedUserId, body.CreatedById);
    }

    [Fact]
    public async Task GetTicketById_ExistingTicket_Returns200Ok()
    {
        var (createStatus, created) = await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(title: "VPN access issue"));

        Assert.Equal(HttpStatusCode.Created, createStatus);
        Assert.NotNull(created);

        var (statusCode, body) = await Client.GetJsonAsync<TicketResponseDto>($"/api/tickets/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal("VPN access issue", body.Title);
    }

    [Fact]
    public async Task GetTicketById_NotFound_Returns404()
    {
        var response = await Client.GetAsync("/api/tickets/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTicket_ValidData_Returns200Ok()
    {
        var (createStatus, created) = await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(title: "Original title"));

        Assert.Equal(HttpStatusCode.Created, createStatus);
        Assert.NotNull(created);

        await AuthTestHelper.AuthenticateAsAgentAsync(Client);

        var updateDto = new UpdateTicketDto
        {
            Title = "Updated title",
            Description = "Updated description with more detail",
            Priority = "High",
            AssignedToId = TestDataSeeder.DefaultAssignedToId
        };

        var (statusCode, body) = await Client.PutJsonAsync<TicketResponseDto>(
            $"/api/tickets/{created.Id}",
            updateDto);

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Equal(updateDto.Title, body.Title);
        Assert.Equal(updateDto.Description, body.Description);
        Assert.Equal(updateDto.Priority, body.Priority);
        Assert.Equal("Open", body.Status);
    }

    [Fact]
    public async Task GetTickets_ReturnsAllCreatedTickets()
    {
        await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(title: "Ticket A"));

        await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(title: "Ticket B"));

        var (statusCode, body) = await Client.GetJsonAsync<TicketListResponseDto>("/api/tickets");

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Equal(2, body.Items.Count);
        Assert.Equal(2, body.TotalCount);
        Assert.Contains(body.Items, t => t.Title == "Ticket A");
        Assert.Contains(body.Items, t => t.Title == "Ticket B");
    }
}
