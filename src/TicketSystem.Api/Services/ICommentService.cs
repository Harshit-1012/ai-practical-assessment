using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Services;

public interface ICommentService
{
    Task<IReadOnlyList<CommentResponseDto>> GetCommentsByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);
    Task<CommentResponseDto> CreateCommentAsync(int ticketId, CreateCommentDto dto, int createdById, CancellationToken cancellationToken = default);
}
