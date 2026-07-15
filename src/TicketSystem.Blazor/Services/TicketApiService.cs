using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public class TicketApiService : ITicketApiService
{
    private readonly ApiClientHelper _api;

    public TicketApiService(ApiClientHelper api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<TicketDto>> GetTicketsAsync(
        TicketSearchCriteria? criteria = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(criteria);
        var tickets = await _api.GetAsync<List<TicketDto>>($"api/tickets{query}", cancellationToken);
        return tickets;
    }

    public Task<TicketDto> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _api.GetAsync<TicketDto>($"api/tickets/{id}", cancellationToken);

    public Task<TicketDto> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default) =>
        _api.PostAsync<TicketDto>("api/tickets", request, cancellationToken);

    public Task<TicketDto> UpdateTicketAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken = default) =>
        _api.PutAsync<TicketDto>($"api/tickets/{id}", request, cancellationToken);

    public Task<TicketDto> ChangeStatusAsync(int id, string newStatus, CancellationToken cancellationToken = default) =>
        _api.PutAsync<TicketDto>($"api/tickets/{id}/status", new { newStatus }, cancellationToken);

    private static string BuildQueryString(TicketSearchCriteria? criteria)
    {
        if (criteria is null)
        {
            return string.Empty;
        }

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(criteria.Keyword))
        {
            parts.Add($"keyword={Uri.EscapeDataString(criteria.Keyword.Trim())}");
        }

        if (!string.IsNullOrWhiteSpace(criteria.Status) && criteria.Status != "All")
        {
            parts.Add($"status={Uri.EscapeDataString(criteria.Status)}");
        }

        return parts.Count == 0 ? string.Empty : "?" + string.Join("&", parts);
    }
}
