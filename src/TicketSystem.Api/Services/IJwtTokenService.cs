using TicketSystem.Api.Models;

namespace TicketSystem.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
