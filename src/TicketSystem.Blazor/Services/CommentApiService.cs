using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public class CommentApiService : ICommentApiService
{
    private readonly ApiClientHelper _api;

    public CommentApiService(ApiClientHelper api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<CommentDto>> GetCommentsAsync(int ticketId, CancellationToken cancellationToken = default)
    {
        var comments = await _api.GetAsync<List<CommentDto>>($"api/tickets/{ticketId}/comments", cancellationToken);
        return comments;
    }

    public Task<CommentDto> CreateCommentAsync(int ticketId, CreateCommentRequest request, CancellationToken cancellationToken = default) =>
        _api.PostAsync<CommentDto>($"api/tickets/{ticketId}/comments", request, cancellationToken);
}
