using Microsoft.EntityFrameworkCore;
using TicketSystem.Api.Data;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Exceptions;
using TicketSystem.Api.Models;
using TicketSystem.Api.Models.Enums;

using TicketSystem.Api.Validation;

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

    public async Task<TicketListResponseDto> GetTicketsAsync(
        TicketListQueryDto query,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(query.Status) && !TicketStatusExtensions.IsValid(query.Status))
        {
            throw new BusinessValidationException("status", "Status must be one of: Open, InProgress, Resolved, Closed, Cancelled.");
        }

        if (!string.IsNullOrWhiteSpace(query.Priority) && !TicketPriorityExtensions.IsValid(query.Priority))
        {
            throw new BusinessValidationException("priority", "Priority must be one of: Low, Medium, High, Critical.");
        }

        if (query.AssignedToId.HasValue && query.UnassignedOnly)
        {
            throw new BusinessValidationException("assignedToId", "Specify either assignedToId or unassignedOnly, not both.");
        }

        if (query.AssignedToId.HasValue)
        {
            await ValidateUserExistsAsync(query.AssignedToId.Value, "assignedToId", cancellationToken);
        }

        var pageNumber = query.PageNumber < TicketListQueryDto.DefaultPageNumber
            ? TicketListQueryDto.DefaultPageNumber
            : query.PageNumber;

        if (query.PageSize < 1)
        {
            throw new BusinessValidationException("pageSize", "PageSize must be at least 1.");
        }

        if (query.PageSize > TicketListQueryDto.MaxPageSize)
        {
            throw new BusinessValidationException("pageSize", $"PageSize cannot exceed {TicketListQueryDto.MaxPageSize}.");
        }

        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "CreatedAt" : query.SortBy;
        var sortDirection = string.IsNullOrWhiteSpace(query.SortDirection) ? "desc" : query.SortDirection;

        if (!IsValidSortBy(sortBy))
        {
            throw new BusinessValidationException("sortBy", "SortBy must be one of: CreatedAt, Priority, Status.");
        }

        if (!IsValidSortDirection(sortDirection))
        {
            throw new BusinessValidationException("sortDirection", "SortDirection must be asc or desc.");
        }

        var ticketQuery = _context.Tickets
            .AsNoTracking()
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            ticketQuery = ticketQuery.Where(t => t.Status == query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.Priority))
        {
            ticketQuery = ticketQuery.Where(t => t.Priority == query.Priority);
        }

        if (query.UnassignedOnly)
        {
            ticketQuery = ticketQuery.Where(t => t.AssignedToId == null);
        }
        else if (query.AssignedToId.HasValue)
        {
            ticketQuery = ticketQuery.Where(t => t.AssignedToId == query.AssignedToId);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var term = query.Keyword.Trim();
            ticketQuery = ticketQuery.Where(t =>
                t.Title.Contains(term) || t.Description.Contains(term));
        }

        var tickets = await ticketQuery.ToListAsync(cancellationToken);
        var sorted = ApplySorting(tickets, sortBy, sortDirection);
        var totalCount = sorted.Count;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)query.PageSize);
        var currentPage = totalPages == 0 ? 1 : Math.Min(pageNumber, totalPages);

        var pageItems = sorted
            .Skip((currentPage - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(MapToResponse)
            .ToList();

        return new TicketListResponseDto
        {
            Items = pageItems,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = currentPage
        };
    }

    private static bool IsValidSortBy(string sortBy) =>
        sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase)
        || sortBy.Equals("Priority", StringComparison.OrdinalIgnoreCase)
        || sortBy.Equals("Status", StringComparison.OrdinalIgnoreCase);

    private static bool IsValidSortDirection(string sortDirection) =>
        sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
        || sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

    private static List<Ticket> ApplySorting(List<Ticket> tickets, string sortBy, string sortDirection)
    {
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        if (sortBy.Equals("Priority", StringComparison.OrdinalIgnoreCase))
        {
            return descending
                ? tickets.OrderByDescending(t => GetPriorityOrder(t.Priority)).ThenByDescending(t => t.CreatedAt).ToList()
                : tickets.OrderBy(t => GetPriorityOrder(t.Priority)).ThenByDescending(t => t.CreatedAt).ToList();
        }

        if (sortBy.Equals("Status", StringComparison.OrdinalIgnoreCase))
        {
            return descending
                ? tickets.OrderByDescending(t => GetStatusOrder(t.Status)).ThenByDescending(t => t.CreatedAt).ToList()
                : tickets.OrderBy(t => GetStatusOrder(t.Status)).ThenByDescending(t => t.CreatedAt).ToList();
        }

        return descending
            ? tickets.OrderByDescending(t => t.CreatedAt).ToList()
            : tickets.OrderBy(t => t.CreatedAt).ToList();
    }

    private static int GetPriorityOrder(string priority) => priority switch
    {
        nameof(TicketPriority.Low) => 0,
        nameof(TicketPriority.Medium) => 1,
        nameof(TicketPriority.High) => 2,
        nameof(TicketPriority.Critical) => 3,
        _ => 99
    };

    private static int GetStatusOrder(string status) => status switch
    {
        nameof(TicketStatus.Open) => 0,
        nameof(TicketStatus.InProgress) => 1,
        nameof(TicketStatus.Resolved) => 2,
        nameof(TicketStatus.Closed) => 3,
        nameof(TicketStatus.Cancelled) => 4,
        _ => 99
    };

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

        var title = InputValidation.RequireTrimmedNonWhitespace(dto.Title, "title");
        var description = InputValidation.RequireTrimmedNonWhitespace(dto.Description, "description");

        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            Title = title,
            Description = description,
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

        if (ticket.Status is nameof(TicketStatus.Closed) or nameof(TicketStatus.Cancelled))
        {
            throw new BusinessValidationException(
                "status",
                $"Tickets in {ticket.Status} status cannot be edited.");
        }

        if (dto.AssignedToId.HasValue)
        {
            await ValidateUserExistsAsync(dto.AssignedToId.Value, "assignedToId", cancellationToken);
        }

        ticket.Title = InputValidation.RequireTrimmedNonWhitespace(dto.Title, "title");
        ticket.Description = InputValidation.RequireTrimmedNonWhitespace(dto.Description, "description");
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
