using System.Net;
using TicketSystem.Api.DTOs;
using TicketSystem.Tests.Helpers;

namespace TicketSystem.Tests.IntegrationTests;

public class CommentIntegrationTests : IntegrationTestBase
{
    public CommentIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateComment_ValidData_Returns201Created()
    {
        var (_, ticket) = await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(title: "Comment target ticket"));

        Assert.NotNull(ticket);

        var commentDto = TestDataSeeder.CreateValidCommentDto("Please reset my password.");

        var (statusCode, body) = await Client.PostJsonAsync<CommentResponseDto>(
            $"/api/tickets/{ticket.Id}/comments",
            commentDto);

        Assert.Equal(HttpStatusCode.Created, statusCode);
        Assert.NotNull(body);
        Assert.True(body.Id > 0);
        Assert.Equal(ticket.Id, body.TicketId);
        Assert.Equal(commentDto.Message, body.Message);
        Assert.Equal(TestDataSeeder.DefaultAuthenticatedUserId, body.CreatedById);
    }

    [Fact]
    public async Task GetComments_AfterCreation_ReturnsCommentList()
    {
        var (_, ticket) = await Client.PostJsonAsync<TicketResponseDto>(
            "/api/tickets",
            TestDataSeeder.CreateValidTicketDto(title: "Ticket with comments"));

        Assert.NotNull(ticket);

        await Client.PostJsonAsync<CommentResponseDto>(
            $"/api/tickets/{ticket.Id}/comments",
            TestDataSeeder.CreateValidCommentDto("First comment"));

        await Client.PostJsonAsync<CommentResponseDto>(
            $"/api/tickets/{ticket.Id}/comments",
            TestDataSeeder.CreateValidCommentDto("Second comment"));

        var (statusCode, body) = await Client.GetJsonAsync<List<CommentResponseDto>>(
            $"/api/tickets/{ticket.Id}/comments");

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(body);
        Assert.Equal(2, body.Count);
        Assert.Equal("First comment", body[0].Message);
        Assert.Equal("Second comment", body[1].Message);
    }

    [Fact]
    public async Task CreateComment_TicketNotFound_Returns404()
    {
        var (statusCode, _) = await Client.PostJsonAsync<CommentResponseDto>(
            "/api/tickets/99999/comments",
            TestDataSeeder.CreateValidCommentDto());

        Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }
}
