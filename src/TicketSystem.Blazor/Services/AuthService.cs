using System.Net.Http.Json;
using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public interface IAuthService
{
    Task<AuthenticatedUser?> GetCurrentUserAsync();
    Task<bool> LoginAsync(int userId, CancellationToken cancellationToken = default);
    Task LogoutAsync();
    Task InitializeAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        ITokenStorageService tokenStorage,
        CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
        _authStateProvider = authStateProvider;
    }

    public Task<AuthenticatedUser?> GetCurrentUserAsync() => _tokenStorage.GetUserAsync();

    public async Task<bool> LoginAsync(int userId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest { UserId = userId },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
        if (loginResponse is null || string.IsNullOrWhiteSpace(loginResponse.Token))
        {
            return false;
        }

        await _tokenStorage.SetTokenAsync(loginResponse.Token);
        await _tokenStorage.SetUserAsync(new AuthenticatedUser
        {
            UserId = loginResponse.UserId,
            Name = loginResponse.Name,
            Email = loginResponse.Email,
            Role = loginResponse.Role
        });

        _authStateProvider.NotifyAuthenticationStateChanged();
        return true;
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.ClearAsync();
        _authStateProvider.NotifyAuthenticationStateChanged();
    }

    public async Task InitializeAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();
        var user = await _tokenStorage.GetUserAsync();

        if ((user is not null || !string.IsNullOrWhiteSpace(token)) && !AuthTokenHelper.IsTokenValid(token))
        {
            await _tokenStorage.ClearAsync();
        }

        _authStateProvider.NotifyAuthenticationStateChanged();
    }
}
