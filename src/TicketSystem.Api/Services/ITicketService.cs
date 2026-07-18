using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Services;

public interface ITicketService
{
    Task<TicketListResponseDto> GetTicketsAsync(TicketListQueryDto query, CancellationToken cancellationToken = default);
    Task<TicketResponseDto> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, int createdById, CancellationToken cancellationToken = default);
    Task<TicketResponseDto> UpdateTicketAsync(int id, UpdateTicketDto dto, CancellationToken cancellationToken = default);
    Task<TicketResponseDto> ChangeStatusAsync(int id, ChangeTicketStatusDto dto, CancellationToken cancellationToken = default);
}
