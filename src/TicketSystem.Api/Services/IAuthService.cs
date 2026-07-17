using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(int userId, CancellationToken cancellationToken = default);
}
