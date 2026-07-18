using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public interface ITicketApiService
{
    Task<TicketListResult> GetTicketsAsync(TicketSearchCriteria? criteria = null, CancellationToken cancellationToken = default);
    Task<TicketDto> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TicketDto> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketDto> UpdateTicketAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketDto> ChangeStatusAsync(int id, string newStatus, CancellationToken cancellationToken = default);
}
