using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponseDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserResponseDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
}
