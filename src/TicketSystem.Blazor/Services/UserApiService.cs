using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public class UserApiService : IUserApiService
{
    private readonly ApiClientHelper _api;

    public UserApiService(ApiClientHelper api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _api.GetAsync<List<UserDto>>("api/users", cancellationToken);
        return users;
    }

    public Task<UserDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _api.GetAsync<UserDto>($"api/users/{id}", cancellationToken);
}
