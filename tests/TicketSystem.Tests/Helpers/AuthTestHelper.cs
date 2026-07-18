using System.Net.Http.Headers;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Tests.Helpers;

public static class AuthTestHelper
{
    public static Task AuthenticateAsUserAsync(HttpClient client) =>
        AuthenticateAsUserAsync(client, TestDataSeeder.DefaultAuthenticatedUserId);

    public static Task AuthenticateAsAgentAsync(HttpClient client) =>
        AuthenticateAsUserAsync(client, TestDataSeeder.DefaultAgentUserId);

    public static async Task AuthenticateAsUserAsync(
        HttpClient client,
        int userId)
    {
        var (statusCode, loginResponse) = await client.PostJsonAsync<LoginResponseDto>(
            "/api/auth/login",
            new LoginDto { UserId = userId });

        if (statusCode != System.Net.HttpStatusCode.OK || loginResponse is null)
        {
            throw new InvalidOperationException($"Failed to authenticate test user {userId}. Status: {statusCode}");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);
    }
}
