using Microsoft.EntityFrameworkCore;
using TicketSystem.Api.Data;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Exceptions;
using TicketSystem.Api.Models;

using TicketSystem.Api.Validation;

namespace TicketSystem.Api.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CommentResponseDto>> GetCommentsByTicketIdAsync(
        int ticketId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTicketExistsAsync(ticketId, cancellationToken);

        var comments = await _context.Comments
            .AsNoTracking()
            .Include(c => c.CreatedBy)
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return comments.Select(MapToResponse).ToList();
    }

    public async Task<CommentResponseDto> CreateCommentAsync(
        int ticketId,
        CreateCommentDto dto,
        int createdById,
        CancellationToken cancellationToken = default)
    {
        await EnsureTicketExistsAsync(ticketId, cancellationToken);
        await EnsureUserExistsAsync(createdById, cancellationToken);

        var message = InputValidation.RequireTrimmedNonWhitespace(dto.Message, "message");

        var comment = new Comment
        {
            TicketId = ticketId,
            Message = message,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Comments
            .AsNoTracking()
            .Include(c => c.CreatedBy)
            .FirstAsync(c => c.Id == comment.Id, cancellationToken);

        return MapToResponse(created);
    }

    private async Task EnsureTicketExistsAsync(int ticketId, CancellationToken cancellationToken)
    {
        var exists = await _context.Tickets.AnyAsync(t => t.Id == ticketId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Ticket not found.");
        }
    }

    private async Task EnsureUserExistsAsync(int userId, CancellationToken cancellationToken)
    {
        var exists = await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        if (!exists)
        {
            throw new BusinessValidationException("createdById", $"User with id {userId} does not exist.");
        }
    }

    private static CommentResponseDto MapToResponse(Comment comment) =>
        new()
        {
            Id = comment.Id,
            TicketId = comment.TicketId,
            Message = comment.Message,
            CreatedById = comment.CreatedById,
            CreatedByName = comment.CreatedBy.Name,
            CreatedAt = comment.CreatedAt
        };
}
