using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public class TicketApiService : ITicketApiService
{
    private readonly ApiClientHelper _api;

    public TicketApiService(ApiClientHelper api)
    {
        _api = api;
    }

    public async Task<TicketListResult> GetTicketsAsync(
        TicketSearchCriteria? criteria = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(criteria);
        return await _api.GetAsync<TicketListResult>($"api/tickets{query}", cancellationToken);
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
        criteria ??= new TicketSearchCriteria();

        var parts = new List<string>
        {
            $"pageNumber={criteria.PageNumber}",
            $"pageSize={criteria.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(criteria.Keyword))
        {
            parts.Add($"keyword={Uri.EscapeDataString(criteria.Keyword.Trim())}");
        }

        if (!string.IsNullOrWhiteSpace(criteria.Status) && criteria.Status != "All")
        {
            parts.Add($"status={Uri.EscapeDataString(criteria.Status)}");
        }

        if (!string.IsNullOrWhiteSpace(criteria.Priority) && criteria.Priority != "All")
        {
            parts.Add($"priority={Uri.EscapeDataString(criteria.Priority)}");
        }

        if (criteria.UnassignedOnly)
        {
            parts.Add("unassignedOnly=true");
        }
        else if (criteria.AssignedToId.HasValue)
        {
            parts.Add($"assignedToId={criteria.AssignedToId.Value}");
        }

        if (!string.IsNullOrWhiteSpace(criteria.SortBy) && !criteria.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
        {
            parts.Add($"sortBy={Uri.EscapeDataString(criteria.SortBy)}");
        }

        if (!string.IsNullOrWhiteSpace(criteria.SortDirection) && !criteria.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            parts.Add($"sortDirection={Uri.EscapeDataString(criteria.SortDirection)}");
        }

        return "?" + string.Join("&", parts);
    }
}
