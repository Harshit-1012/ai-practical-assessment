using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public interface IUserApiService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
}
