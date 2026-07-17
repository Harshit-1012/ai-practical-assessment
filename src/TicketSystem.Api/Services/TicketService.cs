using Microsoft.EntityFrameworkCore;
using TicketSystem.Api.Data;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Exceptions;
using TicketSystem.Api.Models;
using TicketSystem.Api.Models.Enums;

namespace TicketSystem.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;
    private readonly ITicketStateMachine _stateMachine;

    public TicketService(AppDbContext context, ITicketStateMachine stateMachine)
    {
        _context = context;
        _stateMachine = stateMachine;
    }

    public async Task<IReadOnlyList<TicketResponseDto>> GetTicketsAsync(
        string? status,
        string? keyword,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(status) && !TicketStatusExtensions.IsValid(status))
        {
            throw new BusinessValidationException("status", "Status must be one of: Open, InProgress, Resolved, Closed, Cancelled.");
        }

        var query = _context.Tickets
            .AsNoTracking()
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(t => t.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim();
            query = query.Where(t =>
                t.Title.Contains(term) || t.Description.Contains(term));
        }

        var tickets = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return tickets.Select(MapToResponse).ToList();
    }

    public async Task<TicketResponseDto> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketWithDetailsQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticket is null)
        {
            throw new NotFoundException($"Ticket not found.");
        }

        return MapToDetailResponse(ticket);
    }

    public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, int createdById, CancellationToken cancellationToken = default)
    {
        ValidatePriority(dto.Priority);
        await ValidateUserExistsAsync(createdById, "createdById", cancellationToken);

        if (dto.AssignedToId.HasValue)
        {
            await ValidateUserExistsAsync(dto.AssignedToId.Value, "assignedToId", cancellationToken);
        }

        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Priority = dto.Priority,
            Status = TicketStatus.Open.ToString(),
            AssignedToId = dto.AssignedToId,
            CreatedById = createdById,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetTicketByIdAsync(ticket.Id, cancellationToken);
    }

    public async Task<TicketResponseDto> UpdateTicketAsync(int id, UpdateTicketDto dto, CancellationToken cancellationToken = default)
    {
        ValidatePriority(dto.Priority);

        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found.");
        }

        if (dto.AssignedToId.HasValue)
        {
            await ValidateUserExistsAsync(dto.AssignedToId.Value, "assignedToId", cancellationToken);
        }

        ticket.Title = dto.Title.Trim();
        ticket.Description = dto.Description.Trim();
        ticket.Priority = dto.Priority;
        ticket.AssignedToId = dto.AssignedToId;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetTicketByIdAsync(id, cancellationToken);
    }

    public async Task<TicketResponseDto> ChangeStatusAsync(int id, ChangeTicketStatusDto dto, CancellationToken cancellationToken = default)
    {
        if (!TicketStatusExtensions.IsValid(dto.NewStatus))
        {
            throw new BusinessValidationException("newStatus", "Status must be one of: Open, InProgress, Resolved, Closed, Cancelled.");
        }

        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found.");
        }

        var currentStatus = TicketStatusExtensions.Parse(ticket.Status);
        var newStatus = TicketStatusExtensions.Parse(dto.NewStatus);

        _stateMachine.ValidateTransition(currentStatus, newStatus);

        ticket.Status = newStatus.ToString();
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetTicketByIdAsync(id, cancellationToken);
    }

    private IQueryable<Ticket> GetTicketWithDetailsQuery() =>
        _context.Tickets
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Include(t => t.Comments)
                .ThenInclude(c => c.CreatedBy);

    private static void ValidatePriority(string priority)
    {
        if (!TicketPriorityExtensions.IsValid(priority))
        {
            throw new BusinessValidationException("priority", "Priority must be one of: Low, Medium, High, Critical.");
        }
    }

    private async Task ValidateUserExistsAsync(int userId, string fieldName, CancellationToken cancellationToken)
    {
        var exists = await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        if (!exists)
        {
            throw new BusinessValidationException(fieldName, $"User with id {userId} does not exist.");
        }
    }

    private static TicketResponseDto MapToResponse(Ticket ticket) =>
        new()
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            AssignedToId = ticket.AssignedToId,
            AssignedToName = ticket.AssignedTo?.Name,
            CreatedById = ticket.CreatedById,
            CreatedByName = ticket.CreatedBy.Name,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };

    private static TicketResponseDto MapToDetailResponse(Ticket ticket)
    {
        var response = MapToResponse(ticket);
        response.Comments = ticket.Comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                TicketId = c.TicketId,
                Message = c.Message,
                CreatedById = c.CreatedById,
                CreatedByName = c.CreatedBy.Name,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        return response;
    }
}
