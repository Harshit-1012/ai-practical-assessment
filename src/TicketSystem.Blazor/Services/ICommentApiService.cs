using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public interface ICommentApiService
{
    Task<IReadOnlyList<CommentDto>> GetCommentsAsync(int ticketId, CancellationToken cancellationToken = default);
    Task<CommentDto> CreateCommentAsync(int ticketId, CreateCommentRequest request, CancellationToken cancellationToken = default);
}
